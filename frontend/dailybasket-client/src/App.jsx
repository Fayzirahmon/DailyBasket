import { useEffect, useMemo, useState } from "react";
import { customersApi } from "./api/customersApi";
import AppLayout from "./components/AppLayout";
import { ErrorMessage, LoadingState } from "./components/StatusMessage";
import Login from "./pages/Login";
import Cart from "./pages/Cart";
import Categories from "./pages/Categories";
import Customers from "./pages/Customers";
import Dashboard from "./pages/Dashboard";
import Orders from "./pages/Orders";
import Products from "./pages/Products";

export default function App() {
  const [session, setSession] = useState(() => {
    const saved = localStorage.getItem("db_session");
    return saved ? JSON.parse(saved) : null;
  });
  const [activePage, setActivePage] = useState("dashboard");
  const [customers, setCustomers] = useState([]);
  const [activeCustomerId, setActiveCustomerId] = useState(null);
  const [loadingCustomers, setLoadingCustomers] = useState(true);
  const [customerError, setCustomerError] = useState("");

  const activeCustomer = useMemo(() => {
    if (session?.role === "customer" && session.customer) {
      return session.customer;
    }
    return customers.find((customer) => customer.customerId === activeCustomerId) ?? null;
  }, [customers, activeCustomerId, session]);

  const loadCustomers = async () => {
    try {
      setCustomerError("");
      const data = await customersApi.getAll();
      setCustomers(data);
    } catch (error) {
      setCustomerError(error.message);
    } finally {
      setLoadingCustomers(false);
    }
  };

  useEffect(() => {
    loadCustomers();
  }, []);

  useEffect(() => {
    if (session) {
      if (session.role === "admin") {
        setActiveCustomerId(null);
        setActivePage((current) =>
          ["dashboard", "products", "categories", "customers", "orders"].includes(current) ? current : "dashboard"
        );
      } else {
        setActiveCustomerId(session.customerId);
        setActivePage((current) =>
          ["products", "cart", "orders"].includes(current) ? current : "products"
        );
      }
    }
  }, [session]);

  const handleLogout = () => {
    localStorage.removeItem("db_session");
    setSession(null);
  };

  const renderPage = () => {
    switch (activePage) {
      case "products":
        return <Products activeCustomerId={activeCustomerId} role={session?.role} />;
      case "categories":
        return <Categories />;
      case "customers":
        return (
          <Customers
            customers={customers}
            reloadCustomers={loadCustomers}
          />
        );
      case "cart":
        return <Cart activeCustomer={activeCustomer} />;
      case "orders":
        return <Orders activeCustomer={activeCustomer} role={session?.role} />;
      default:
        return <Dashboard activeCustomer={activeCustomer} />;
    }
  };

  if (!session) {
    return <Login onLogin={setSession} />;
  }

  if (loadingCustomers) {
    return <LoadingState label="Starting DailyBasket..." />;
  }

  return (
    <AppLayout
      activePage={activePage}
      onNavigate={setActivePage}
      customers={customers}
      activeCustomerId={activeCustomerId}
      onCustomerChange={setActiveCustomerId}
      session={session}
      onLogout={handleLogout}
    >
      <ErrorMessage message={customerError} />
      {renderPage()}
    </AppLayout>
  );
}
