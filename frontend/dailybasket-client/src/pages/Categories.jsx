/**
 * ====================================================================
 * Component: frontend/dailybasket-client/src/pages/Categories.jsx
 * Layer: Presentation Layer (React Component Page)
 * Purpose: Provides administrator CRUD view for organizing inventory categories.
 * ====================================================================
 */

import { useEffect, useState } from "react";
import { Edit, Plus, Trash2 } from "lucide-react";
import { categoriesApi } from "../api/categoriesApi";
import ConfirmDialog from "../components/ConfirmDialog";
import FormField from "../components/FormField";
import Modal from "../components/Modal";
import PageHeader from "../components/PageHeader";
import { EmptyState, ErrorMessage, LoadingState, SuccessMessage } from "../components/StatusMessage";

const blankCategory = {
  categoryName: "",
  description: ""
};

/**
 * Categories management page component for Admin roles.
 * Provides views to create, read, update, and delete product categories.
 * Enforces relational constraints (e.g., categories containing active products cannot be deleted).
 * 
 * @returns {JSX.Element} The rendered Categories view.
 */
export default function Categories() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [formOpen, setFormOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState(null);
  const [form, setForm] = useState(blankCategory);
  const [formErrors, setFormErrors] = useState({});
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [busy, setBusy] = useState(false);

  /**
   * Fetches the category list from backend APIs.
   */
  const loadCategories = async () => {
    try {
      setLoading(true);
      setError("");
      setCategories(await categoriesApi.getAll());
    } catch (loadError) {
      setError(loadError.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

  /**
   * Triggers the create modal open with clean state.
   */
  const openCreate = () => {
    setEditingCategory(null);
    setForm(blankCategory);
    setFormErrors({});
    setFormOpen(true);
  };

  /**
   * Triggers the edit modal open, preloading the target category attributes.
   * 
   * @param {Object} category - The category to edit.
   */
  const openEdit = (category) => {
    setEditingCategory(category);
    setForm({
      categoryName: category.categoryName,
      description: category.description ?? ""
    });
    setFormErrors({});
    setFormOpen(true);
  };

  /**
   * Validates target category fields.
   * 
   * @returns {boolean} True if validation passes, otherwise false.
   */
  const validate = () => {
    const errors = {};
    if (!form.categoryName.trim()) errors.categoryName = "Category name is required.";
    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  /**
   * Submits category creations and updates to backend API.
   * 
   * @param {React.FormEvent} event - React form submit event.
   */
  const submitForm = async (event) => {
    event.preventDefault();
    if (!validate()) return;

    try {
      setBusy(true);
      setError("");
      setSuccess("");
      if (editingCategory) {
        await categoriesApi.update(editingCategory.categoryId, form);
        setSuccess("Category updated.");
      } else {
        await categoriesApi.create(form);
        setSuccess("Category created.");
      }
      setFormOpen(false);
      await loadCategories();
    } catch (submitError) {
      setError(submitError.message);
    } finally {
      setBusy(false);
    }
  };

  /**
   * Deletes a category and manages loader state.
   */
  const deleteCategory = async () => {
    if (!deleteTarget) return;

    try {
      setBusy(true);
      setError("");
      setSuccess("");
      await categoriesApi.remove(deleteTarget.categoryId);
      setSuccess("Category deleted.");
      setDeleteTarget(null);
      await loadCategories();
    } catch (deleteError) {
      setError(deleteError.message);
    } finally {
      setBusy(false);
    }
  };

  if (loading) {
    return <LoadingState />;
  }

  return (
    <>
      <PageHeader
        title="Categories"
        description="Maintain grocery groups used by the product catalog."
        action={
          <button type="button" className="btn-primary" onClick={openCreate}>
            <Plus size={16} aria-hidden="true" />
            Add Category
          </button>
        }
      />

      <ErrorMessage message={error} />
      <SuccessMessage message={success} />

      {categories.length === 0 ? (
        <EmptyState title="No categories found" description="Create the first category to organize products." />
      ) : (
        <div className="overflow-hidden rounded-md border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Name</th>
                <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Description</th>
                <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Products</th>
                <th className="px-4 py-3 text-right text-xs font-bold uppercase text-slate-500">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {categories.map((category) => (
                <tr key={category.categoryId}>
                  <td className="px-4 py-3 text-sm font-semibold text-slate-950">{category.categoryName}</td>
                  <td className="max-w-xl px-4 py-3 text-sm text-slate-600">{category.description || "No description."}</td>
                  <td className="px-4 py-3 text-sm text-slate-600">{category.productCount}</td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-2">
                      <button
                        type="button"
                        title="Edit category"
                        className="focus-ring flex h-9 w-9 items-center justify-center rounded-md border border-slate-300 text-slate-700 hover:bg-slate-50"
                        onClick={() => openEdit(category)}
                      >
                        <Edit size={16} aria-hidden="true" />
                      </button>
                      <button
                        type="button"
                        title="Delete category"
                        className="focus-ring flex h-9 w-9 items-center justify-center rounded-md border border-rose-200 text-rose-600 hover:bg-rose-50"
                        onClick={() => setDeleteTarget(category)}
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
      )}

      {formOpen ? (
        <Modal title={editingCategory ? "Edit Category" : "Add Category"} onClose={() => setFormOpen(false)}>
          <form className="space-y-4" onSubmit={submitForm}>
            <FormField label="Category name" error={formErrors.categoryName}>
              <input
                className="field"
                value={form.categoryName}
                onChange={(event) => setForm((current) => ({ ...current, categoryName: event.target.value }))}
              />
            </FormField>
            <FormField label="Description">
              <textarea
                className="field min-h-24"
                value={form.description}
                onChange={(event) => setForm((current) => ({ ...current, description: event.target.value }))}
              />
            </FormField>
            <div className="flex justify-end gap-3">
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
          title="Delete Category"
          message={`Delete ${deleteTarget.categoryName}? Categories with products cannot be removed.`}
          onCancel={() => setDeleteTarget(null)}
          onConfirm={deleteCategory}
          busy={busy}
        />
      ) : null}
    </>
  );
}
