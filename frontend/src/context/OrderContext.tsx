"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
  type ReactNode,
} from "react";

import { useAuth } from "./AuthContext";
import type { Order } from "../types/order";
import type { OrderResponse } from "../types/orderResponse";
import { listOrdersRequest } from "../services/orderService";
import { getFriendlyErrorMessage } from "../utils/apiErrors";

type OrderContextType = {
  orders: Order[];
  isOrderLoading: boolean;
  orderError: string | null;
  refreshOrders: () => Promise<void>;
  getOrderByNumber: (orderNumber: string) => Order | undefined;
};

type OrderState = {
  status: "loading" | "success" | "error";
  orders: Order[];
  errorMessage: string | null;
};

// oturumdan kullancii kimligini bulur
type OrderProviderProps = {
  children: ReactNode;
};

// o kullaniciya ait siparis verisini yonetir
type UserOrderProviderProps = OrderProviderProps & {
  userId: string | null;
  isAuthLoading: boolean;
};

const OrderContext = createContext<OrderContextType | undefined>(undefined);

function toOrder(response: OrderResponse): Order {
  const { status } = response;

  if (
    status !== "AwaitingPayment" &&
    status !== "Paid" &&
    status !== "Cancelled"
  ) {
    throw new Error("API desteklenmeyen bir sipariş durumu döndürdü.");
  }

  return { ...response, status };
}

export function OrderProvider({ children }: OrderProviderProps) {
  const { user, isAuthLoading } = useAuth();

  // Oturum kontrolü bitmeden kullanıcıya ait istek başlatmıyoruz.
  const userId = isAuthLoading ? null : user?.id ?? null;

  return (
    <UserOrderProvider
      key={userId ?? "anonymous"}
      userId={userId}
      isAuthLoading={isAuthLoading}
    >
      {children}
    </UserOrderProvider>
  );
}

function UserOrderProvider({
  children,
  userId,
  isAuthLoading,
}: UserOrderProviderProps) {
  const [state, setState] = useState<OrderState>({
    status: "loading",
    orders: [],
    errorMessage: null,
  });

  const latestRequestId = useRef(0);
  const isMounted = useRef(false);

  const loadOrders = useCallback(async (requestId: number) => {
    try {
      const response = await listOrdersRequest();

      // Eski veya artık kullanılmayan bir isteğin sonucunu uygulama.
      if (!isMounted.current || requestId !== latestRequestId.current) {
        return;
      }

      const orders = response.map(toOrder);

      setState({
        status: "success",
        orders,
        errorMessage: null,
      });
    } catch (error) {
      if (!isMounted.current || requestId !== latestRequestId.current) {
        return;
      }

      setState({
        status: "error",
        orders: [],
        errorMessage: getFriendlyErrorMessage(
          error,
          "Siparişlerin yüklenemedi. Lütfen tekrar dene."
        ),
      });
    }
  }, []);

  useEffect(() => {
    isMounted.current = true;
    const requestId = ++latestRequestId.current;

    if (userId !== null) {
      void loadOrders(requestId);
    }

    return () => {
      isMounted.current = false;
      latestRequestId.current += 1;
    };
  }, [userId, loadOrders]);

  const refreshOrders = useCallback(async () => {
    if (userId === null || !isMounted.current) return;

    const requestId = ++latestRequestId.current;

    setState({
      status: "loading",
      orders: [],
      errorMessage: null,
    });

    await loadOrders(requestId);
  }, [userId, loadOrders]);

  function getOrderByNumber(orderNumber: string) {
    return orders.find((order) => order.orderNumber === orderNumber);
  }

  return (
    <OrderContext.Provider
      value={{
        orders: userId === null ? [] : state.orders,
        isOrderLoading:
          isAuthLoading || (userId !== null && state.status === "loading"),
        orderError: userId === null ? null : state.errorMessage,
        refreshOrders,
      }}
    >
      {children}
    </OrderContext.Provider>
  );
}

export function useOrder() {
  const context = useContext(OrderContext);

  if (!context) {
    throw new Error("useOrder, OrderProvider içerisinde kullanılmalıdır.");
  }

  return context;
}
