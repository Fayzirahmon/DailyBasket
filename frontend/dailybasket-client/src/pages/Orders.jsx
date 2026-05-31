/**
 * ====================================================================
 * Component: frontend/dailybasket-client/src/pages/Orders.jsx
 * Layer: Presentation Layer (React Component Page)
 * Purpose: Provides a dual-view order tracking grid (Customer personal history 
 *          and Admin operational orders switcher).
 * ====================================================================
 */

import { useEffect, useState } from "react";
import { ChevronDown, ChevronRight } from "lucide-react";
import { ordersApi } from "../api/ordersApi";
import Currency from "../components/Currency";
import PageHeader from "../components/PageHeader";
import { EmptyState, ErrorMessage, LoadingState, SuccessMessage } from "../components/StatusMessage";

const orderStatuses = ["Pending", "Processing", "Delivered", "Cancelled"];

const statusStyles = {
  Pending: "bg-amber-50 text-amber-700 border-amber-200",
  Processing: "bg-sky-50 text-sky-700 border-sky-200",
  Delivered: "bg-emerald-50 text-emerald-700 border-emerald-200",
  Cancelled: "bg-rose-50 text-rose-700 border-rose-200"
};

/**
 * Orders history and management portal page.
 * Customers view their own personal checkouts, while Admins view all system checkouts,
 * toggle filter states, and edit active order statuses dynamically.
 * 
 * @param {Object} props
 * @param {Object} props.activeCustomer - Active logged-in customer record.
 * @param {string} props.role - User security role ('admin' or 'customer').
 * @returns {JSX.Element} The rendered Orders catalog view.
 */
export default function Orders({ activeCustomer, role }) {
  const [orders, setOrders] = useState([]);
  const [viewMode, setViewMode] = useState("all");
  const [expanded, setExpanded] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [statusSavingId, setStatusSavingId] = useState(null);

  /**
   * Fetches order records based on current role boundaries and filters.
   */
  const loadOrders = async () => {
    try {
      setLoading(true);
      setError("");
      let data;
      if (viewMode === "customer" && activeCustomer) {
        data = await ordersApi.getByCustomer(activeCustomer.customerId);
      } else {
        data = await ordersApi.getAll();
        if (viewMode === "active") {
          data = data.filter((order) => order.status === "Pending" || order.status === "Processing");
        }
      }
      setOrders(data);
    } catch (loadError) {
      setError(loadError.message);
    } finally {
      setLoading(false);
    }
  };

  // Adjust view bounds depending on user profile role
  useEffect(() => {
    if (role === "customer") {
      setViewMode("customer");
    }
  }, [role]);

  // Reload lists when view configuration updates
  useEffect(() => {
    loadOrders();
  }, [viewMode, activeCustomer]);

  // Self-clears success toast messages
  useEffect(() => {
    if (success) {
      const timer = setTimeout(() => setSuccess(""), 4500);
      return () => clearTimeout(timer);
    }
  }, [success]);

  // Self-clears operational error banners
  useEffect(() => {
    if (error) {
      const timer = setTimeout(() => setError(""), 6000);
      return () => clearTimeout(timer);
    }
  }, [error]);

  /**
   * Expands or collapses item-level table details in the grid.
   * 
   * @param {number} orderId - Target order database ID.
   */
  const toggleExpanded = (orderId) => {
    setExpanded((current) => ({ ...current, [orderId]: !current[orderId] }));
  };

  /**
   * Triggers background order status change updates (Admin only).
   * 
   * @param {Object} order - Target order entity to modify.
   * @param {string} status - New target status text.
   */
  const updateStatus = async (order, status) => {
    if (order.status === status) {
      return;
    }

    try {
      setError("");
      setSuccess("");
      setStatusSavingId(order.orderId);
      const updatedOrder = await ordersApi.updateStatus(order.orderId, status);
      setOrders((current) =>
        current.map((currentOrder) => (currentOrder.orderId === updatedOrder.orderId ? updatedOrder : currentOrder))
      );
      setSuccess(`Order #${updatedOrder.orderId} changed to ${updatedOrder.status}.`);
    } catch (statusError) {
      setError(statusError.message);
    } finally {
      setStatusSavingId(null);
    }
  };

  if (loading) {
    return <LoadingState />;
  }

  return (
    <>
      <PageHeader
        title="Orders"
        description="Review checkout records and item-level order details."
        action={
          role !== "customer" ? (
            <div className="inline-flex rounded-md border border-slate-300 bg-white p-1 shadow-sm">
              <button
                type="button"
                className={`rounded px-3 py-1.5 text-sm font-semibold ${
                  viewMode === "all" ? "bg-slate-900 text-white" : "text-slate-600 hover:bg-slate-50"
                }`}
                onClick={() => setViewMode("all")}
              >
                All
              </button>
              <button
                type="button"
                className={`rounded px-3 py-1.5 text-sm font-semibold ${
                  viewMode === "active" ? "bg-slate-900 text-white" : "text-slate-600 hover:bg-slate-50"
                }`}
                onClick={() => setViewMode("active")}
              >
                Active Orders
              </button>
            </div>
          ) : null
        }
      />

      <ErrorMessage message={error} onClose={() => setError("")} />
      <SuccessMessage message={success} onClose={() => setSuccess("")} />

      {orders.length === 0 ? (
        <EmptyState title="No orders found" description="Checkout a cart to create an order." />
      ) : (
        <div className="space-y-3">
          {orders.map((order) => {
            const isExpanded = expanded[order.orderId];
            return (
              <article key={order.orderId} className="rounded-md border border-slate-200 bg-white shadow-sm">
                <div className="flex flex-col gap-4 px-4 py-4 sm:flex-row sm:items-center sm:justify-between">
                  <button
                    type="button"
                    onClick={() => toggleExpanded(order.orderId)}
                    className="focus-ring flex min-w-0 items-center gap-3 rounded-md text-left"
                  >
                    {isExpanded ? (
                      <ChevronDown size={18} aria-hidden="true" />
                    ) : (
                      <ChevronRight size={18} aria-hidden="true" />
                    )}
                    <div>
                      <p className="text-sm font-bold text-slate-950">Order #{order.orderId}</p>
                      <p className="text-xs text-slate-500">
                        {order.customerName} / {new Date(order.orderDate).toLocaleString()}
                      </p>
                    </div>
                  </button>

                  <div className="flex flex-wrap items-center gap-3 sm:justify-end">
                    <span className="text-xs font-bold uppercase text-slate-500">Status</span>
                    {role === "admin" ? (
                      <select
                        className={`rounded-md border px-2 py-1.5 text-sm font-bold cursor-pointer ${
                          statusStyles[order.status] ?? "border-slate-200 bg-slate-50 text-slate-700"
                        }`}
                        value={order.status}
                        disabled={statusSavingId === order.orderId}
                        onChange={(event) => updateStatus(order, event.target.value)}
                      >
                        {orderStatuses.map((status) => (
                          <option key={status} value={status}>
                            {status}
                          </option>
                        ))}
                      </select>
                    ) : (
                      <span
                        className={`rounded-md border px-2.5 py-1 text-sm font-bold ${
                          statusStyles[order.status] ?? "border-slate-200 bg-slate-50 text-slate-700"
                        }`}
                      >
                        {order.status}
                      </span>
                    )}
                    <span className="text-sm font-bold text-slate-950">
                      <Currency value={order.totalAmount} />
                    </span>
                  </div>
                </div>

                {isExpanded ? (
                  <div className="border-t border-slate-200 px-4 py-4">
                    <p className="mb-3 text-sm text-slate-600">{order.deliveryAddress}</p>
                    <div className="overflow-hidden rounded-md border border-slate-200">
                      <table className="min-w-full divide-y divide-slate-200">
                        <thead className="bg-slate-50">
                          <tr>
                            <th className="px-3 py-2 text-left text-xs font-bold uppercase text-slate-500">Product</th>
                            <th className="px-3 py-2 text-left text-xs font-bold uppercase text-slate-500">Quantity</th>
                            <th className="px-3 py-2 text-left text-xs font-bold uppercase text-slate-500">Unit Price</th>
                            <th className="px-3 py-2 text-left text-xs font-bold uppercase text-slate-500">Line Total</th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-100">
                          {order.items.map((item) => (
                            <tr key={item.orderItemId}>
                              <td className="px-3 py-2 text-sm font-semibold text-slate-950">{item.productName}</td>
                              <td className="px-3 py-2 text-sm text-slate-600">{item.quantity}</td>
                              <td className="px-3 py-2 text-sm text-slate-600">
                                <Currency value={item.unitPrice} />
                              </td>
                              <td className="px-3 py-2 text-sm font-bold text-slate-950">
                                <Currency value={item.lineTotal} />
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                ) : null}
              </article>
            );
          })}
        </div>
      )}
    </>
  );
}
