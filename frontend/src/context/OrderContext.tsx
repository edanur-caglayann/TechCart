"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import { useAuth } from "./AuthContext";
import type { Order } from "../types/order";
import { listOrdersRequest } from "../services/orderService";

type OrderContextType = {
  orders: Order[];
  isOrderLoading: boolean;
  refreshOrders: () => Promise<void>;
};

const OrderContext = createContext<OrderContextType | undefined>(undefined);

type OrderProviderProps = {
  children: ReactNode;
};

export function OrderProvider({ children }: OrderProviderProps) {
  const { isAuthenticated } = useAuth();

  const [orders, setOrders] = useState<Order[]>([]);
  const [isOrderLoading, setIsOrderLoading] = useState(true);

  useEffect(() => {
    if (!isAuthenticated) {
      setOrders([]);
      setIsOrderLoading(false);
      return;
    }

    refreshOrders();
  }, [isAuthenticated]);

  async function refreshOrders() {
    setIsOrderLoading(true);
    try {
      const result = await listOrdersRequest();
      setOrders(result as Order[]);
    } finally {
      setIsOrderLoading(false);
    }
  }

  return (
    <OrderContext.Provider value={{ orders, isOrderLoading, refreshOrders }}>
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