/**
 * ====================================================================
 * Component: frontend/dailybasket-client/src/pages/Cart.jsx
 * Layer: Presentation Layer (React Component Page)
 * Purpose: Provides shopping cart interfaces, quantity editing,
 *          item removals, and checkout submission.
 * ====================================================================
 */

import { useEffect, useRef, useState } from "react";
import { ShoppingBag, Trash2 } from "lucide-react";
import { cartApi } from "../api/cartApi";
import { ordersApi } from "../api/ordersApi";
import ConfirmDialog from "../components/ConfirmDialog";
import Currency from "../components/Currency";
import FormField from "../components/FormField";
import Modal from "../components/Modal";
import PageHeader from "../components/PageHeader";
import { EmptyState, ErrorMessage, LoadingState, SuccessMessage } from "../components/StatusMessage";

/**
 * Cart page component managing the customer's grocery cart.
 * Employs a local optimistic UI update strategy with debounced API execution 
 * to provide highly responsive quantity updates without manual "save" buttons.
 * 
 * @param {Object} props
 * @param {Object} props.activeCustomer - The currently logged-in customer profile.
 * @returns {JSX.Element} The rendered Cart and Checkout page views.
 */
export default function Cart({ activeCustomer }) {
  const [cart, setCart] = useState(null);
  const [quantities, setQuantities] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [removeTarget, setRemoveTarget] = useState(null);
  const [checkoutOpen, setCheckoutOpen] = useState(false);
  const [deliveryAddress, setDeliveryAddress] = useState("");
  const [formError, setFormError] = useState("");
  const [busy, setBusy] = useState(false);
  
  // Holds unique timer IDs mapped by cartItemId to debounce rapid quantity arrow updates
  const debounceTimers = useRef({});

  /**
   * Fetches active cart data from backend repository.
   */
  const loadCart = async () => {
    if (!activeCustomer) {
      return;
    }

    try {
      setLoading(true);
      setError("");
      const data = await cartApi.getByCustomer(activeCustomer.customerId);
      setCart(data);
      setQuantities(Object.fromEntries(data.items.map((item) => [item.cartItemId, item.quantity])));
    } catch (loadError) {
      setError(loadError.message);
    } finally {
      setLoading(false);
    }
  };

  // Triggers reload whenever customer session changes
  useEffect(() => {
    setSuccess("");
    setDeliveryAddress(activeCustomer?.address ?? "");
    loadCart();
  }, [activeCustomer]);

  // Self-clears success notification after delay
  useEffect(() => {
    if (success) {
      const timer = setTimeout(() => setSuccess(""), 4500);
      return () => clearTimeout(timer);
    }
  }, [success]);

  // Self-clears error messages after delay
  useEffect(() => {
    if (error) {
      const timer = setTimeout(() => setError(""), 6000);
      return () => clearTimeout(timer);
    }
  }, [error]);

  // Clean up debounced timers on component destroy
  useEffect(() => {
    return () => {
      // Clean up all active timers on unmount
      Object.values(debounceTimers.current).forEach((timerId) => clearTimeout(timerId));
    };
  }, []);

  /**
   * Performs debounced backend quantity updates to optimize network requests.
   * 
   * @param {number} cartItemId - The database target item identifier.
   * @param {number} quantity - The target quantity.
   */
  const debounceUpdate = (cartItemId, quantity) => {
    if (debounceTimers.current[cartItemId]) {
      clearTimeout(debounceTimers.current[cartItemId]);
    }

    debounceTimers.current[cartItemId] = setTimeout(async () => {
      try {
        setError("");
        setSuccess("");
        await cartApi.update(cartItemId, quantity);
        // Refresh silently from the server to guarantee consistency
        const data = await cartApi.getByCustomer(activeCustomer.customerId);
        setCart(data);
      } catch (updateError) {
        setError(updateError.message);
      }
    }, 400);
  };

  /**
   * Optimistically updates UI state and schedules backend sync on quantity changes.
   * 
   * @param {Object} item - The current cart item being edited.
   * @param {string} valStr - The raw numeric text value.
   */
  const handleQuantityChange = (item, valStr) => {
    const val = Number(valStr);
    setQuantities((current) => ({ ...current, [item.cartItemId]: valStr }));

    if (!valStr || val < 1 || val > item.stockQuantity) {
      return;
    }

    // Optimistic UI state updates for frictionless user interaction
    setCart((currentCart) => {
      if (!currentCart) return null;
      const updatedItems = currentCart.items.map((currentItem) => {
        if (currentItem.cartItemId === item.cartItemId) {
          const newLineTotal = val * currentItem.unitPrice;
          return {
            ...currentItem,
            quantity: val,
            lineTotal: newLineTotal
          };
        }
        return currentItem;
      });

      const totalItems = updatedItems.reduce((acc, curr) => acc + curr.quantity, 0);
      const totalAmount = updatedItems.reduce((acc, curr) => acc + curr.lineTotal, 0);

      return {
        ...currentCart,
        items: updatedItems,
        totalItems,
        totalAmount
      };
    });

    debounceUpdate(item.cartItemId, val);
  };

  /**
   * Deletes a cart item and reloads the interface.
   */
  const removeItem = async () => {
    if (!removeTarget) return;

    try {
      setBusy(true);
      setError("");
      setSuccess("");
      await cartApi.remove(removeTarget.cartItemId);
      setSuccess("Cart item removed.");
      setRemoveTarget(null);
      await loadCart();
    } catch (removeError) {
      setError(removeError.message);
    } finally {
      setBusy(false);
    }
  };

  /**
   * Submits checkout transaction request and creates the order records.
   * 
   * @param {React.FormEvent} event - The standard React form submission event.
   */
  const checkout = async (event) => {
    event.preventDefault();
    if (!deliveryAddress.trim()) {
      setFormError("Delivery address is required.");
      return;
    }

    try {
      setBusy(true);
      setError("");
      setSuccess("");
      setFormError("");
      const order = await ordersApi.checkout(activeCustomer.customerId, {
        deliveryAddress
      });
      setCheckoutOpen(false);
      setSuccess(`Checkout completed. Order #${order.orderId} created.`);
      await loadCart();
    } catch (checkoutError) {
      setError(checkoutError.message);
    } finally {
      setBusy(false);
    }
  };

  if (!activeCustomer) {
    return <EmptyState title="No active customer" description="Create or select a customer to manage a cart." />;
  }

  if (loading) {
    return <LoadingState />;
  }

  return (
    <>
      <PageHeader
        title="Cart"
        description={`Cart for ${activeCustomer.fullName}.`}
        action={
          <button
            type="button"
            className="btn-primary"
            disabled={!cart || cart.items.length === 0}
            onClick={() => setCheckoutOpen(true)}
          >
            <ShoppingBag size={16} aria-hidden="true" />
            Checkout
          </button>
        }
      />

      <ErrorMessage message={error} onClose={() => setError("")} />
      <SuccessMessage message={success} onClose={() => setSuccess("")} />

      {!cart || cart.items.length === 0 ? (
        <EmptyState title="Cart is empty" description="Add products from the product catalog." />
      ) : (
        <div className="grid gap-5 xl:grid-cols-[1fr_320px]">
          <div className="overflow-hidden rounded-md border border-slate-200 bg-white shadow-sm">
            <table className="min-w-full divide-y divide-slate-200">
              <thead className="bg-slate-50">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Product</th>
                  <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Price</th>
                  <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Quantity</th>
                  <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Line Total</th>
                  <th className="px-4 py-3 text-right text-xs font-bold uppercase text-slate-500">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {cart.items.map((item) => (
                  <tr key={item.cartItemId}>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-3">
                        <div className="h-14 w-14 overflow-hidden rounded-md bg-slate-200">
                          {item.imageUrl ? (
                            <img src={item.imageUrl} alt={item.productName} className="h-full w-full object-cover" />
                          ) : null}
                        </div>
                        <div>
                          <p className="text-sm font-bold text-slate-950">{item.productName}</p>
                          <p className="text-xs text-slate-500">{item.stockQuantity} in stock</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-sm font-semibold text-slate-700">
                      <Currency value={item.unitPrice} />
                    </td>
                    <td className="px-4 py-3">
                      <input
                        className="field w-24"
                        type="number"
                        min="1"
                        max={item.stockQuantity}
                        value={quantities[item.cartItemId] ?? item.quantity}
                        onChange={(event) =>
                          handleQuantityChange(item, event.target.value)
                        }
                      />
                    </td>
                    <td className="px-4 py-3 text-sm font-bold text-slate-950">
                      <Currency value={item.lineTotal} />
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex justify-end gap-2">
                        <button
                          type="button"
                          title="Remove item"
                          className="focus-ring flex h-9 w-9 items-center justify-center rounded-md border border-rose-200 text-rose-600 hover:bg-rose-50 cursor-pointer"
                          onClick={() => setRemoveTarget(item)}
                        >
                          <Trash2 size={16} aria-hidden="true" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <aside className="rounded-md border border-slate-200 bg-white p-5 shadow-sm">
            <h2 className="text-lg font-bold text-slate-950">Summary</h2>
            <div className="mt-4 space-y-3 text-sm text-slate-600">
              <div className="flex justify-between">
                <span>Items</span>
                <span className="font-bold text-slate-950">{cart.totalItems}</span>
              </div>
              <div className="flex justify-between border-t border-slate-200 pt-3">
                <span>Total</span>
                <span className="text-lg font-bold text-slate-950">
                  <Currency value={cart.totalAmount} />
                </span>
              </div>
            </div>
          </aside>
        </div>
      )}

      {checkoutOpen ? (
        <Modal title="Checkout" onClose={() => setCheckoutOpen(false)}>
          <form className="space-y-4" onSubmit={checkout}>
            <FormField label="Delivery address" error={formError}>
              <textarea
                className="field min-h-24"
                value={deliveryAddress}
                onChange={(event) => setDeliveryAddress(event.target.value)}
              />
            </FormField>
            <div className="rounded-md bg-slate-50 p-4 text-sm text-slate-600">
              <div className="flex justify-between">
                <span>Total amount</span>
                <span className="font-bold text-slate-950">
                  <Currency value={cart.totalAmount} />
                </span>
              </div>
            </div>
            <div className="flex justify-end gap-3">
              <button type="button" className="btn-secondary" onClick={() => setCheckoutOpen(false)} disabled={busy}>
                Cancel
              </button>
              <button type="submit" className="btn-primary" disabled={busy}>
                {busy ? "Checking out..." : "Place Order"}
              </button>
            </div>
          </form>
        </Modal>
      ) : null}

      {removeTarget ? (
        <ConfirmDialog
          title="Remove Cart Item"
          message={`Remove ${removeTarget.productName} from the cart?`}
          confirmLabel="Remove"
          onCancel={() => setRemoveTarget(null)}
          onConfirm={removeItem}
          busy={busy}
        />
      ) : null}
    </>
  );
}
