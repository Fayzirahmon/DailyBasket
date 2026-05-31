/**
 * ====================================================================
 * Component: frontend/dailybasket-client/src/pages/Customers.jsx
 * Layer: Presentation Layer (React Component Page)
 * Purpose: Provides administrator CRUD view for registered grocery customers.
 * ====================================================================
 */

import { useEffect, useState } from "react";
import { Edit, Plus, Trash2 } from "lucide-react";
import { customersApi } from "../api/customersApi";
import ConfirmDialog from "../components/ConfirmDialog";
import FormField from "../components/FormField";
import Modal from "../components/Modal";
import PageHeader from "../components/PageHeader";
import { EmptyState, ErrorMessage, SuccessMessage } from "../components/StatusMessage";

const blankCustomer = {
  fullName: "",
  email: "",
  phoneNumber: "",
  address: ""
};

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

/**
 * Customers management page component for Admin roles.
 * Provides views to create, read, update, and delete customer accounts.
 * Enforces email validation and prevents deletion of customers who have active orders or carts.
 * 
 * @param {Object} props
 * @param {Array<Object>} props.customers - List of active customer profiles.
 * @param {Function} props.reloadCustomers - Callback to refresh customer state from server.
 * @returns {JSX.Element} The rendered Customers view.
 */
export default function Customers({ customers, reloadCustomers }) {
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [formOpen, setFormOpen] = useState(false);
  const [editingCustomer, setEditingCustomer] = useState(null);
  const [form, setForm] = useState(blankCustomer);
  const [formErrors, setFormErrors] = useState({});
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [busy, setBusy] = useState(false);

  /**
   * Resets and opens the form modal for adding a new customer.
   */
  const openCreate = () => {
    setEditingCustomer(null);
    setForm(blankCustomer);
    setFormErrors({});
    setFormOpen(true);
  };

  /**
   * Preloads editing customer attributes and opens the modal.
   * 
   * @param {Object} customer - The customer to edit.
   */
  const openEdit = (customer) => {
    setEditingCustomer(customer);
    setForm({
      fullName: customer.fullName,
      email: customer.email,
      phoneNumber: customer.phoneNumber,
      address: customer.address
    });
    setFormErrors({});
    setFormOpen(true);
  };

  /**
   * Helper utility to update form fields in local state.
   * 
   * @param {string} field - The object property key.
   * @param {string} value - The input text value.
   */
  const updateField = (field, value) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  /**
   * Validates form parameters (name, email regex, phone, address).
   * 
   * @returns {boolean} True if validation passes, otherwise false.
   */
  const validate = () => {
    const errors = {};
    if (!form.fullName.trim()) errors.fullName = "Full name is required.";
    if (!emailPattern.test(form.email.trim())) errors.email = "Enter a valid email address.";
    if (!form.phoneNumber.trim()) errors.phoneNumber = "Phone number is required.";
    if (!form.address.trim()) errors.address = "Address is required.";
    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  /**
   * Submits customer account additions or updates to the server.
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
      if (editingCustomer) {
        await customersApi.update(editingCustomer.customerId, form);
        setSuccess("Customer updated.");
      } else {
        await customersApi.create(form);
        setSuccess("Customer created.");
      }
      setFormOpen(false);
      await reloadCustomers();
    } catch (submitError) {
      setError(submitError.message);
    } finally {
      setBusy(false);
    }
  };

  /**
   * Deletes a customer account and manages loader state.
   */
  const deleteCustomer = async () => {
    if (!deleteTarget) return;

    try {
      setBusy(true);
      setError("");
      setSuccess("");
      await customersApi.remove(deleteTarget.customerId);
      setSuccess("Customer deleted.");
      setDeleteTarget(null);
      await reloadCustomers();
    } catch (deleteError) {
      setError(deleteError.message);
    } finally {
      setBusy(false);
    }
  };

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

  return (
    <>
      <PageHeader
        title="Customers"
        description="Manage customer contact details used by carts and orders."
        action={
          <button type="button" className="btn-primary" onClick={openCreate}>
            <Plus size={16} aria-hidden="true" />
            Add Customer
          </button>
        }
      />

      <ErrorMessage message={error} onClose={() => setError("")} />
      <SuccessMessage message={success} onClose={() => setSuccess("")} />

      {customers.length === 0 ? (
        <EmptyState title="No customers found" description="Create a customer before using cart and order workflows." />
      ) : (
        <div className="overflow-hidden rounded-md border border-slate-200 bg-white shadow-sm">
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Customer</th>
                <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Contact</th>
                <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Address</th>
                <th className="px-4 py-3 text-left text-xs font-bold uppercase text-slate-500">Activity</th>
                <th className="px-4 py-3 text-right text-xs font-bold uppercase text-slate-500">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {customers.map((customer) => (
                <tr key={customer.customerId}>
                  <td className="px-4 py-3">
                    <p className="text-sm font-bold text-slate-950">{customer.fullName}</p>
                  </td>
                  <td className="px-4 py-3 text-sm text-slate-600">
                    <p>{customer.email}</p>
                    <p>{customer.phoneNumber}</p>
                  </td>
                  <td className="max-w-md px-4 py-3 text-sm text-slate-600">{customer.address}</td>
                  <td className="px-4 py-3 text-sm text-slate-600">
                    {customer.cartItemCount} cart / {customer.orderCount} orders
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-2">
                      <button
                        type="button"
                        title="Edit customer"
                        className="focus-ring flex h-9 w-9 items-center justify-center rounded-md border border-slate-300 text-slate-700 hover:bg-slate-50 cursor-pointer"
                        onClick={() => openEdit(customer)}
                      >
                        <Edit size={16} aria-hidden="true" />
                      </button>
                      <button
                        type="button"
                        title="Delete customer"
                        className="focus-ring flex h-9 w-9 items-center justify-center rounded-md border border-rose-200 text-rose-600 hover:bg-rose-50"
                        onClick={() => setDeleteTarget(customer)}
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
        <Modal title={editingCustomer ? "Edit Customer" : "Add Customer"} onClose={() => setFormOpen(false)}>
          <form className="space-y-4" onSubmit={submitForm}>
            <div className="grid gap-4 sm:grid-cols-2">
              <FormField label="Full name" error={formErrors.fullName}>
                <input className="field" value={form.fullName} onChange={(event) => updateField("fullName", event.target.value)} />
              </FormField>
              <FormField label="Email" error={formErrors.email}>
                <input className="field" type="email" value={form.email} onChange={(event) => updateField("email", event.target.value)} />
              </FormField>
            </div>
            <FormField label="Phone number" error={formErrors.phoneNumber}>
              <input className="field" value={form.phoneNumber} onChange={(event) => updateField("phoneNumber", event.target.value)} />
            </FormField>
            <FormField label="Address" error={formErrors.address}>
              <textarea className="field min-h-24" value={form.address} onChange={(event) => updateField("address", event.target.value)} />
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
          title="Delete Customer"
          message={`Delete ${deleteTarget.fullName}? Customers with cart items or orders cannot be removed.`}
          onCancel={() => setDeleteTarget(null)}
          onConfirm={deleteCustomer}
          busy={busy}
        />
      ) : null}
    </>
  );
}
