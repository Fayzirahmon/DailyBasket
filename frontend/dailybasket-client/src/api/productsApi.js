import apiClient from "./client";

export const productsApi = {
  getAll: async (categoryId) => {
    const response = await apiClient.get("/products", {
      params: categoryId ? { categoryId } : {}
    });
    return response.data;
  },
  create: async (payload) => {
    const response = await apiClient.post("/products", payload);
    return response.data;
  },
  update: async (id, payload) => {
    await apiClient.put(`/products/${id}`, payload);
  },
  remove: async (id) => {
    await apiClient.delete(`/products/${id}`);
  }
};
