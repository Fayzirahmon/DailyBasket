import apiClient from "./client";

export const customersApi = {
  getAll: async () => {
    const response = await apiClient.get("/customers");
    return response.data;
  },
  create: async (payload) => {
    const response = await apiClient.post("/customers", payload);
    return response.data;
  },
  update: async (id, payload) => {
    await apiClient.put(`/customers/${id}`, payload);
  },
  remove: async (id) => {
    await apiClient.delete(`/customers/${id}`);
  }
};
