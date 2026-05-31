import apiClient from "./client";

export const ordersApi = {
  getAll: async () => {
    const response = await apiClient.get("/orders");
    return response.data;
  },
  getByCustomer: async (customerId) => {
    const response = await apiClient.get(`/orders/customer/${customerId}`);
    return response.data;
  },
  checkout: async (customerId, payload) => {
    const response = await apiClient.post(`/orders/checkout/${customerId}`, payload);
    return response.data;
  },
  updateStatus: async (orderId, status) => {
    const response = await apiClient.put(`/orders/${orderId}/status`, { status });
    return response.data;
  }
};
