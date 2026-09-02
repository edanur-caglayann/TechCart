"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import { useAuth } from "./AuthContext";
import type { Order, OrderStatus } from "../types/order";

// Sipariş oluşturulurken dışarıdan alınacak bilgiler.
type CreateOrderData = Omit<
  Order,
  | "id"
  | "orderNumber"
  | "userEmail"
  | "status"
  | "createdAt"
>;

type OrderContextType = {
  orders: Order[];
  isOrderLoading: boolean;
  createOrder: (
    orderData: CreateOrderData
  ) => Order;
  getOrderByNumber: (
    orderNumber: string
  ) => Order | undefined;
  updateOrderStatus: (
    orderNumber: string,
    status: OrderStatus
  ) => void;
};

const OrderContext = createContext<
  OrderContextType | undefined
>(undefined);

type OrderProviderProps = {
  children: ReactNode;
};

export function OrderProvider({
  children,
}: OrderProviderProps) {
  const { user } = useAuth();

  // Giriş yapan kullanıcının siparişlerini tutar.
  const [orders, setOrders] = useState<Order[]>(
    []
  );

  const [
    isOrderLoading,
    setIsOrderLoading,
  ] = useState(true);

  // Kullanıcıya özel localStorage anahtarını tutar.
  const [storageKey, setStorageKey] = useState<
    string | null
  >(null);

  /*
    Kullanıcı giriş yaptığında kendisine ait
    siparişleri localStorage üzerinden yükler.
  */
  useEffect(() => {
    if (!user) {
      setOrders([]);
      setStorageKey(null);
      setIsOrderLoading(false);
      return;
    }

    setIsOrderLoading(true);

    const userStorageKey =
      `techcart-orders-${user.email}`;

    const savedOrders =
      localStorage.getItem(userStorageKey);

    if (savedOrders) {
      try {
        const parsedOrders: Order[] =
          JSON.parse(savedOrders);

        setOrders(parsedOrders);
      } catch {
        localStorage.removeItem(userStorageKey);
        setOrders([]);
      }
    } else {
      setOrders([]);
    }

    setStorageKey(userStorageKey);
    setIsOrderLoading(false);
  }, [user]);

  /*
    Sipariş listesi değiştiğinde güncel listeyi
    localStorage içerisine kaydeder.
  */
  useEffect(() => {
    if (!storageKey || isOrderLoading) {
      return;
    }

    localStorage.setItem(
      storageKey,
      JSON.stringify(orders)
    );
  }, [
    orders,
    storageKey,
    isOrderLoading,
  ]);

  // Kullanıcıya gösterilecek sipariş numarasını oluşturur.
  function generateOrderNumber() {
    const timePart =
      Date.now().toString().slice(-8);

    const randomPart = Math.floor(
      1000 + Math.random() * 9000
    );

    return `TC-${timePart}-${randomPart}`;
  }

  // Ödeme sonrasında yeni sipariş oluşturur.
  function createOrder(
    orderData: CreateOrderData
  ) {
    if (!user) {
      throw new Error(
        "Sipariş oluşturmak için giriş yapmalısınız."
      );
    }

    const newOrder: Order = {
      ...orderData,
      id: crypto.randomUUID(),
      orderNumber: generateOrderNumber(),
      userEmail: user.email,
      status: "Preparing",
      createdAt: new Date().toISOString(),
    };

    setOrders((currentOrders) => [
      newOrder,
      ...currentOrders,
    ]);

    return newOrder;
  }

  // Sipariş numarasına göre ilgili siparişi bulur.
  function getOrderByNumber(
    orderNumber: string
  ) {
    return orders.find(
      (order) =>
        order.orderNumber === orderNumber
    );
  }

  /*
  Sipariş numarasına göre ilgili siparişin
  durumunu günceller.
  Orders listesini dolasir ve eşleşen sipariş numarasına sahip siparişin 
  sadece status alanini gunceller.
*/
  function updateOrderStatus(
    orderNumber: string,
    status: OrderStatus
  ) {
    setOrders((currentOrders) =>
      currentOrders.map((order) =>
        order.orderNumber === orderNumber
          ? {
            ...order,
            status,
          }
          : order
      )
    );
  }

  return (
    <OrderContext.Provider
      value={{
        orders,
        isOrderLoading,
        createOrder,
        getOrderByNumber,
        updateOrderStatus,
      }}
    >
      {children}
    </OrderContext.Provider>
  );
}

export function useOrder() {
  const context = useContext(OrderContext);

  if (!context) {
    throw new Error(
      "useOrder, OrderProvider içerisinde kullanılmalıdır."
    );
  }

  return context;
}