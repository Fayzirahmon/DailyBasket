import apiClient from "./client";

export const categoriesApi = {
  getAll: async () => {
    const response = await apiClient.get("/categories");
    return response.data;
  },
  create: async (payload) => {
    const response = await apiClient.post("/categories", payload);
    return response.data;
  },
  update: async (id, payload) => {
    await apiClient.put(`/categories/${id}`, payload);
  },
  remove: async (id) => {
    await apiClient.delete(`/categories/${id}`);
  }
};
