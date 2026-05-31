import apiClient from "./client";

export const cartApi = {
  getByCustomer: async (customerId) => {
    const response = await apiClient.get(`/cart/customer/${customerId}`);
    return response.data;
  },
  add: async (payload) => {
    const response = await apiClient.post("/cart", payload);
    return response.data;
  },
  update: async (cartItemId, quantity) => {
    const response = await apiClient.put(`/cart/${cartItemId}`, { quantity });
    return response.data;
  },
  remove: async (cartItemId) => {
    await apiClient.delete(`/cart/${cartItemId}`);
  },
  clear: async (customerId) => {
    await apiClient.delete(`/cart/customer/${customerId}`);
  },
};
