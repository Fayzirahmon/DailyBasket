/**
 * ====================================================================
 * Component: frontend/dailybasket-client/src/pages/Products.jsx
 * Layer: Presentation Layer (React Component Page)
 * Purpose: Provides product list visualization, category filtering,
 *          customer cart additions, and admin inventory controls.
 * ====================================================================
 */

import { useEffect, useMemo, useState } from "react";
import { Edit, Plus, ShoppingCart, Trash2 } from "lucide-react";
import { cartApi } from "../api/cartApi";
import { categoriesApi } from "../api/categoriesApi";
import { productsApi } from "../api/productsApi";
import ConfirmDialog from "../components/ConfirmDialog";
import Currency from "../components/Currency";
import FormField from "../components/FormField";
import Modal from "../components/Modal";
import PageHeader from "../components/PageHeader";
import { EmptyState, ErrorMessage, LoadingState, SuccessMessage, ToastMessage } from "../components/StatusMessage";

const blankProduct = {
  categoryId: "",
  productName: "",
  description: "",
  price: "",
  stockQuantity: "",
  imageUrl: "",
  isAvailable: true
};

/**
 * Products list component rendering grocery cards.
 * Customers view the products and click 'Add to Cart', whilst Admins are presented
 * with buttons to add, update, or remove products in their inventories.
 * 
 * @param {Object} props
 * @param {number} props.activeCustomerId - Active logged-in customer's ID context.
 * @param {string} props.role - User security role context ('admin' or 'customer').
 * @returns {JSX.Element} The rendered Products view.
 */
export default function Products({ activeCustomerId, role }) {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [categoryFilter, setCategoryFilter] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [toast, setToast] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [form, setForm] = useState(blankProduct);
  const [formErrors, setFormErrors] = useState({});
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [busy, setBusy] = useState(false);

  // Computes active category name context to render in headers
  const selectedCategoryName = useMemo(
    () => categories.find((category) => category.categoryId === Number(categoryFilter))?.categoryName ?? "All categories",
    [categories, categoryFilter]
  );

  /**
   * Loads category catalogs and products from the API endpoints.
   */
  const loadData = async () => {
    try {
      setLoading(true);
      setError("");
      const [categoryData, productData] = await Promise.all([
        categoriesApi.getAll(),
        productsApi.getAll(categoryFilter ? Number(categoryFilter) : undefined)
      ]);
      setCategories(categoryData);
      setProducts(productData);
    } catch (loadError) {
      setError(loadError.message);
    } finally {
      setLoading(false);
    }
  };

  // Reload products automatically when filters are adjusted
  useEffect(() => {
    loadData();
  }, [categoryFilter]);

  // Clean up and display status toast messages
  useEffect(() => {
    if (!toast) {
      return undefined;
    }

    const timeoutId = window.setTimeout(() => setToast(null), 3500);
    return () => window.clearTimeout(timeoutId);
  }, [toast]);

  // Self-clears success state
  useEffect(() => {
    if (success) {
      const timer = setTimeout(() => setSuccess(""), 4500);
      return () => clearTimeout(timer);
    }
  }, [success]);

  // Self-clears error state
  useEffect(() => {
    if (error) {
      const timer = setTimeout(() => setError(""), 6000);
      return () => clearTimeout(timer);
    }
  }, [error]);

  /**
   * Resets and triggers the add-product modal form.
   */
  const openCreateForm = () => {
    setEditingProduct(null);
    setForm({ ...blankProduct, categoryId: categories[0]?.categoryId ?? "" });
    setFormErrors({});
    setFormOpen(true);
  };

  /**
   * Preloads selected product attributes and opens the edit modal.
   * 
   * @param {Object} product - The product entity to edit.
   */
  const openEditForm = (product) => {
    setEditingProduct(product);
    setForm({
      categoryId: product.categoryId,
      productName: product.productName,
      description: product.description ?? "",
      price: product.price,
      stockQuantity: product.stockQuantity,
      imageUrl: product.imageUrl ?? "",
      isAvailable: product.isAvailable
    });
    setFormErrors({});
    setFormOpen(true);
  };

  /**
   * Helper utility to update form fields in local state.
   * 
   * @param {string} field - Object attribute key.
   * @param {any} value - Assigned form value.
   */
  const updateField = (field, value) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const validate = () => {
    const errors = {};
    if (!form.categoryId) errors.categoryId = "Category is required.";
    if (!form.productName.trim()) errors.productName = "Product name is required.";
    if (!form.price || Number(form.price) <= 0) errors.price = "Price must be greater than 0.";
    if (form.stockQuantity === "" || Number(form.stockQuantity) < 0) errors.stockQuantity = "Stock cannot be negative.";
    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const submitForm = async (event) => {
    event.preventDefault();
    if (!validate()) return;

    const payload = {
      ...form,
      categoryId: Number(form.categoryId),
      price: Number(form.price),
      stockQuantity: Number(form.stockQuantity)
    };

    try {
      setBusy(true);
      setError("");
      setSuccess("");
      if (editingProduct) {
        await productsApi.update(editingProduct.productId, payload);
        setSuccess("Product updated.");
      } else {
        await productsApi.create(payload);
        setSuccess("Product created.");
      }
      setFormOpen(false);
      await loadData();
    } catch (submitError) {
      setError(submitError.message);
    } finally {
      setBusy(false);
    }
  };

  const deleteProduct = async () => {
    if (!deleteTarget) return;

    try {
      setBusy(true);
      setError("");
      setSuccess("");
      await productsApi.remove(deleteTarget.productId);
      setSuccess("Product deleted.");
      setDeleteTarget(null);
      await loadData();
    } catch (deleteError) {
      setError(deleteError.message);
    } finally {
      setBusy(false);
    }
  };

  const addToCart = async (product) => {
    if (!activeCustomerId) {
      const message = "Select a customer before adding cart items.";
      setError(message);
      setToast({ type: "error", message });
      return;
    }

    try {
      setError("");
      setSuccess("");
      await cartApi.add({
        customerId: activeCustomerId,
        productId: product.productId,
        quantity: 1
      });
      const message = `${product.productName} added to cart.`;
      setSuccess(message);
      setToast({ type: "success", message });
    } catch (cartError) {
      setError(cartError.message);
      setToast({ type: "error", message: cartError.message });
    }
  };

  if (loading) {
    return <LoadingState />;
  }

  return (
    <>
      <PageHeader
        title="Products"
        description={`Showing ${products.length} products in ${selectedCategoryName}.`}
        action={
          role === "admin" ? (
            <button type="button" className="btn-primary" onClick={openCreateForm}>
              <Plus size={16} aria-hidden="true" />
              Add Product
            </button>
          ) : null
        }
      />

      <ErrorMessage message={error} onClose={() => setError("")} />
      <SuccessMessage message={success} onClose={() => setSuccess("")} />

      <div className="mb-4 flex max-w-xs flex-col gap-1">
        <label htmlFor="categoryFilter" className="text-sm font-semibold text-slate-700">
          Category filter
        </label>
        <select
          id="categoryFilter"
          className="field"
          value={categoryFilter}
          onChange={(event) => setCategoryFilter(event.target.value)}
        >
          <option value="">All categories</option>
          {categories.map((category) => (
            <option key={category.categoryId} value={category.categoryId}>
              {category.categoryName}
            </option>
          ))}
        </select>
      </div>

      {products.length === 0 ? (
        <EmptyState title="No products found" description="Create a product or choose another category filter." />
      ) : (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {products.map((product) => (
            <article key={product.productId} className="rounded-md border border-slate-200 bg-white shadow-sm">
              <div className="aspect-[16/9] overflow-hidden rounded-t-md bg-slate-200">
                {product.imageUrl ? (
                  <img
                    src={product.imageUrl}
                    alt={product.productName}
                    className="h-full w-full object-cover"
                    loading="lazy"
                  />
                ) : (
                  <div className="flex h-full items-center justify-center text-sm font-semibold text-slate-500">
                    No image
                  </div>
                )}
              </div>

              <div className="p-4">
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <h2 className="text-base font-bold text-slate-950">{product.productName}</h2>
                    <p className="mt-1 text-xs font-semibold uppercase text-slate-500">
                      {product.categoryName}
                    </p>
                  </div>
                  <span
                    className={`rounded-md px-2 py-1 text-xs font-bold ${
                      product.isAvailable ? "bg-emerald-50 text-emerald-700" : "bg-slate-100 text-slate-500"
                    }`}
                  >
                    {product.isAvailable ? "Available" : "Unavailable"}
                  </span>
                </div>

                <p className="mt-3 min-h-10 text-sm text-slate-600">{product.description || "No description."}</p>

                <div className="mt-4 flex items-end justify-between">
                  <div>
                    <p className="text-xl font-bold text-slate-950">
                      <Currency value={product.price} />
                    </p>
                    <p className="text-sm text-slate-500">{product.stockQuantity} in stock</p>
                  </div>
                  <div className="flex gap-2">
                    {role !== "admin" ? (
                      <button
                        type="button"
                        title="Add to cart"
                        className="focus-ring flex h-10 w-10 items-center justify-center rounded-md bg-emerald-600 text-white hover:bg-emerald-700 disabled:bg-slate-300"
                        disabled={!product.isAvailable || product.stockQuantity < 1}
                        onClick={() => addToCart(product)}
                      >
                        <ShoppingCart size={17} aria-hidden="true" />
                      </button>
                    ) : (
                      <>
                        <button
                          type="button"
                          title="Edit product"
                          className="focus-ring flex h-10 w-10 items-center justify-center rounded-md border border-slate-300 bg-white text-slate-700 hover:bg-slate-50"
                          onClick={() => openEditForm(product)}
                        >
                          <Edit size={17} aria-hidden="true" />
                        </button>
                        <button
                          type="button"
                          title="Delete product"
                          className="focus-ring flex h-10 w-10 items-center justify-center rounded-md border border-rose-200 bg-white text-rose-600 hover:bg-rose-50"
                          onClick={() => setDeleteTarget(product)}
                        >
                          <Trash2 size={17} aria-hidden="true" />
                        </button>
                      </>
                    )}
                  </div>
                </div>
              </div>
            </article>
          ))}
        </div>
      )}

      {formOpen ? (
        <Modal title={editingProduct ? "Edit Product" : "Add Product"} onClose={() => setFormOpen(false)}>
          <form className="space-y-4" onSubmit={submitForm}>
            <div className="grid gap-4 sm:grid-cols-2">
              <FormField label="Product name" error={formErrors.productName}>
                <input className="field" value={form.productName} onChange={(event) => updateField("productName", event.target.value)} />
              </FormField>
              <FormField label="Category" error={formErrors.categoryId}>
                <select className="field" value={form.categoryId} onChange={(event) => updateField("categoryId", event.target.value)}>
                  <option value="">Select category</option>
                  {categories.map((category) => (
                    <option key={category.categoryId} value={category.categoryId}>
                      {category.categoryName}
                    </option>
                  ))}
                </select>
              </FormField>
              <FormField label="Price" error={formErrors.price}>
                <input className="field" type="number" min="0.01" step="0.01" value={form.price} onChange={(event) => updateField("price", event.target.value)} />
              </FormField>
              <FormField label="Stock quantity" error={formErrors.stockQuantity}>
                <input className="field" type="number" min="0" value={form.stockQuantity} onChange={(event) => updateField("stockQuantity", event.target.value)} />
              </FormField>
            </div>

            <FormField label="Image URL">
              <input className="field" value={form.imageUrl} onChange={(event) => updateField("imageUrl", event.target.value)} />
            </FormField>

            <FormField label="Description">
              <textarea className="field min-h-24" value={form.description} onChange={(event) => updateField("description", event.target.value)} />
            </FormField>

            <label className="flex items-center gap-2 text-sm font-semibold text-slate-700">
              <input
                type="checkbox"
                checked={form.isAvailable}
                onChange={(event) => updateField("isAvailable", event.target.checked)}
                className="h-4 w-4 rounded border-slate-300 text-emerald-600"
              />
              Available
            </label>

            <div className="flex justify-end gap-3 pt-2">
              <button type="button" className="btn-secondary" onClick={() => setFormOpen(false)} disabled={busy}>
                Cancel
              </button>
              <button type="submit" className="btn-primary" disabled={busy}>
                {busy ? "Saving..." : "Save"}
              </button>
            </div>
          </form>
        </Modal>
      ) : null}

      {deleteTarget ? (
        <ConfirmDialog
          title="Delete Product"
          message={`Delete ${deleteTarget.productName}?`}
          onCancel={() => setDeleteTarget(null)}
          onConfirm={deleteProduct}
          busy={busy}
        />
      ) : null}

      <ToastMessage type={toast?.type} message={toast?.message} onClose={() => setToast(null)} />
    </>
  );
}
