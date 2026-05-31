import axios from "axios";

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5075/api",
  headers: {
    "Content-Type": "application/json"
  }
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const data = error.response?.data;
    const message =
      data?.message ||
      (data?.errors ? Object.values(data.errors).flat().join(" ") : null) ||
      "Request failed. Please try again.";

    return Promise.reject(new Error(message));
  }
);

export default apiClient;
