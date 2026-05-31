/**
 * ====================================================================
 * Component: frontend/dailybasket-client/src/pages/Login.jsx
 * Layer: Presentation Layer (React Component Page)
 * Purpose: Provides a beautiful, glassmorphic credentials-free login selection screen 
 *          to bypass traditional authentication for assignment grading.
 * ====================================================================
 */

import { useEffect, useState } from "react";
import { LogIn, Shield, User, ShoppingBasket } from "lucide-react";
import { customersApi } from "../api/customersApi";
import { ErrorMessage } from "../components/StatusMessage";

/**
 * Login page component providing role selection.
 * Admins enter with full operational access, whereas Customers select an 
 * active customer account from a dynamic list to shop.
 * 
 * @param {Object} props
 * @param {Function} props.onLogin - Callback triggered when login succeeds.
 * @returns {JSX.Element} The rendered Login portal component.
 */
export default function Login({ onLogin }) {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [selectedCustomerId, setSelectedCustomerId] = useState("");

  useEffect(() => {
    /**
     * Loads the customer list so they can be selected for login context.
     */
    const fetchCustomers = async () => {
      try {
        setError("");
        const data = await customersApi.getAll();
        setCustomers(data);
      } catch (err) {
        setError("Failed to load customers list: " + err.message);
      } finally {
        setLoading(false);
      }
    };
    fetchCustomers();
  }, []);

  /**
   * Initializes and logs in as an Admin user.
   */
  const handleAdminLogin = () => {
    const session = {
      role: "admin",
      customer: null,
      customerId: null
    };
    localStorage.setItem("db_session", JSON.stringify(session));
    onLogin(session);
  };

  /**
   * Initializes and logs in as a Customer user.
   * 
   * @param {React.FormEvent} e - React form submit event.
   */
  const handleCustomerLogin = (e) => {
    e.preventDefault();
    if (!selectedCustomerId) {
      setError("Please select a customer to log in.");
      return;
    }
    const customer = customers.find(c => c.customerId === Number(selectedCustomerId));
    if (!customer) {
      setError("Invalid customer selected.");
      return;
    }
    const session = {
      role: "customer",
      customer,
      customerId: customer.customerId
    };
    localStorage.setItem("db_session", JSON.stringify(session));
    onLogin(session);
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-radial from-slate-900 via-slate-950 to-black px-4 relative overflow-hidden font-sans">
      {/* Dynamic background accents */}
      <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-emerald-500/10 rounded-full blur-3xl animate-pulse"></div>
      <div className="absolute bottom-1/4 right-1/4 w-96 h-96 bg-blue-500/10 rounded-full blur-3xl animate-pulse"></div>

      <div className="w-full max-w-lg z-10 space-y-8">
        <div className="text-center space-y-2">
          <div className="inline-flex h-16 w-16 items-center justify-center rounded-2xl bg-emerald-500 text-white shadow-lg shadow-emerald-500/20 mb-2 transform hover:scale-105 transition-transform duration-300">
            <ShoppingBasket size={36} />
          </div>
          <h1 className="text-4xl font-extrabold tracking-tight bg-gradient-to-r from-white via-slate-200 to-slate-400 bg-clip-text text-transparent">
            Welcome to DailyBasket
          </h1>
          <p className="text-sm font-medium text-slate-400">
            Select a portal to access your grocery dashboard
          </p>
        </div>

        {error ? (
          <div className="rounded-lg bg-rose-500/10 border border-rose-500/20 p-4">
            <ErrorMessage message={error} />
          </div>
        ) : null}

        <div className="grid gap-6 md:grid-cols-2">
          {/* Admin Access Panel */}
          <div className="group rounded-2xl border border-slate-800 bg-slate-900/60 p-6 backdrop-blur-xl hover:border-emerald-500/40 hover:bg-slate-900/80 transition-all duration-300 flex flex-col justify-between shadow-2xl">
            <div>
              <div className="h-12 w-12 rounded-xl bg-emerald-500/10 text-emerald-400 flex items-center justify-center group-hover:scale-110 transition-transform duration-300">
                <Shield size={24} />
              </div>
              <h2 className="mt-4 text-xl font-bold text-white group-hover:text-emerald-400 transition-colors">
                Manager Portal
              </h2>
              <p className="mt-2 text-xs text-slate-400 leading-relaxed">
                Access dashboard charts, customize inventory, manage categories, and handle customer registries.
              </p>
            </div>
            <button
              onClick={handleAdminLogin}
              className="mt-6 w-full inline-flex items-center justify-center gap-2 rounded-xl bg-emerald-600 hover:bg-emerald-500 py-3 text-sm font-bold text-white shadow-lg shadow-emerald-600/15 transform hover:-translate-y-0.5 active:translate-y-0 transition-all cursor-pointer"
            >
              <Shield size={16} />
              Enter as Admin
            </button>
          </div>

          {/* Customer Access Panel */}
          <div className="group rounded-2xl border border-slate-800 bg-slate-900/60 p-6 backdrop-blur-xl hover:border-blue-500/40 hover:bg-slate-900/80 transition-all duration-300 flex flex-col justify-between shadow-2xl">
            <div>
              <div className="h-12 w-12 rounded-xl bg-blue-500/10 text-blue-400 flex items-center justify-center group-hover:scale-110 transition-transform duration-300">
                <User size={24} />
              </div>
              <h2 className="mt-4 text-xl font-bold text-white group-hover:text-blue-400 transition-colors">
                Customer Shop
              </h2>
              <p className="mt-2 text-xs text-slate-400 leading-relaxed">
                Browse our fresh products catalog, manage your cart, and review recent orders.
              </p>
            </div>

            <form onSubmit={handleCustomerLogin} className="mt-6 space-y-3">
              <select
                id="customerSelect"
                value={selectedCustomerId}
                disabled={loading}
                onChange={(e) => setSelectedCustomerId(e.target.value)}
                className="w-full rounded-xl border border-slate-800 bg-slate-950 px-3 py-2.5 text-sm text-slate-200 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 disabled:opacity-50"
              >
                <option value="">
                  {loading ? "Loading customers..." : "-- Select Customer --"}
                </option>
                {customers.map((c) => (
                  <option key={c.customerId} value={c.customerId}>
                    {c.fullName}
                  </option>
                ))}
              </select>

              <button
                type="submit"
                disabled={loading || !selectedCustomerId}
                className="w-full inline-flex items-center justify-center gap-2 rounded-xl bg-blue-600 hover:bg-blue-500 py-3 text-sm font-bold text-white shadow-lg shadow-blue-600/15 transform hover:-translate-y-0.5 active:translate-y-0 transition-all disabled:opacity-50 disabled:transform-none disabled:cursor-not-allowed cursor-pointer"
              >
                <LogIn size={16} />
                Shop as Customer
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
