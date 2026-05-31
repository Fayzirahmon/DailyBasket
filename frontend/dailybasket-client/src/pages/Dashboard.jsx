/**
 * ====================================================================
 * Component: frontend/dailybasket-client/src/pages/Dashboard.jsx
 * Layer: Presentation Layer (React Component Page)
 * Purpose: Provides administrator system status overview including stats,
 *          low-stock alerts, order summaries, and visual charts.
 * ====================================================================
 */

import { useEffect, useState } from "react";
import { ClipboardList, Package, Tags, Users, PackageCheck, AlertTriangle } from "lucide-react";
import { cartApi } from "../api/cartApi";
import { categoriesApi } from "../api/categoriesApi";
import { customersApi } from "../api/customersApi";
import { ordersApi } from "../api/ordersApi";
import { productsApi } from "../api/productsApi";
import Currency from "../components/Currency";
import PageHeader from "../components/PageHeader";
import { ErrorMessage, LoadingState } from "../components/StatusMessage";
import RevenueByCategoryBarChart from "../components/RevenueByCategoryBarChart";
import RevenueOverTimeLineChart from "../components/RevenueOverTimeLineChart";

/**
 * Dashboard page component for Admin roles.
 * Gathers system-wide analytical metrics concurrently using Promise.all
 * and renders interactive statistical cards and financial graphs.
 * 
 * @param {Object} props
 * @param {Object} props.activeCustomer - The currently selected customer (used for contextual cart metrics).
 * @returns {JSX.Element} The rendered Dashboard view.
 */
export default function Dashboard({ activeCustomer }) {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    /**
     * Executes concurrent promises to pull system statistics from the Web API.
     */
    const load = async () => {
      try {
        setLoading(true);
        setError("");
        const [categories, products, customers, orders, cart] = await Promise.all([
          categoriesApi.getAll(),
          productsApi.getAll(),
          customersApi.getAll(),
          ordersApi.getAll(),
          activeCustomer ? cartApi.getByCustomer(activeCustomer.customerId) : null
        ]);

        setStats({
          categories: categories.length,
          products: products.length,
          customers: customers.length,
          orders: orders.length,
          availableProducts: products.filter((product) => product.isAvailable).length,
          lowStock: products.filter((product) => product.stockQuantity <= 10).length,
          cart: cart ?? { totalItems: 0, totalAmount: 0 },
          rawCategories: categories,
          rawOrders: orders,
          rawProducts: products
        });
      } catch (loadError) {
        setError(loadError.message);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [activeCustomer]);

  if (loading) {
    return <LoadingState />;
  }

  const statCards = [
    { label: "Products", value: stats.products, icon: Package, tone: "text-emerald-700 bg-emerald-50" },
    { label: "Available Products", value: stats.availableProducts, icon: PackageCheck, tone: "text-teal-700 bg-teal-50" },
    { label: "Low Stock Items", value: stats.lowStock, icon: AlertTriangle, tone: "text-rose-700 bg-rose-50" },
    { label: "Categories", value: stats.categories, icon: Tags, tone: "text-sky-700 bg-sky-50" },
    { label: "Customers", value: stats.customers, icon: Users, tone: "text-indigo-700 bg-indigo-50" },
    { label: "Orders", value: stats.orders, icon: ClipboardList, tone: "text-amber-700 bg-amber-50" }
  ];

  return (
    <>
      <PageHeader
        title="Dashboard"
        description="Inventory, customer, cart, and order activity for the grocery system."
      />
      <ErrorMessage message={error} />

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {statCards.map((card) => {
          const Icon = card.icon;
          return (
            <div key={card.label} className="rounded-md border border-slate-200 bg-white p-5 shadow-sm hover:shadow-md transition-shadow duration-200">
              <div className="flex items-center justify-between">
                <p className="text-sm font-semibold text-slate-500">{card.label}</p>
                <span className={`flex h-10 w-10 items-center justify-center rounded-md ${card.tone}`}>
                  <Icon size={20} aria-hidden="true" />
                </span>
              </div>
              <p className="mt-4 text-3xl font-bold text-slate-950">{card.value}</p>
            </div>
          );
        })}
      </div>

      <div className="mt-6 grid gap-4 xl:grid-cols-2">
        <RevenueByCategoryBarChart orders={stats.rawOrders} products={stats.rawProducts} />
        <RevenueOverTimeLineChart orders={stats.rawOrders} />
      </div>
    </>
  );
}
