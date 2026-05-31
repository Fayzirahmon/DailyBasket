import { X } from "lucide-react";

export function LoadingState({ label = "Loading data..." }) {
  return (
    <div className="rounded-md border border-slate-200 bg-white p-6 text-sm font-medium text-slate-600 shadow-sm">
      {label}
    </div>
  );
}

export function ErrorMessage({ message, onClose }) {
  if (!message) {
    return null;
  }

  return (
    <div className="relative mb-4 rounded-md border border-rose-200 bg-rose-50 px-4 py-3 pr-10 text-sm font-semibold text-rose-700 shadow-sm animate-fade-in">
      <p>{message}</p>
      {onClose ? (
        <button
          type="button"
          className="absolute right-2 top-2 flex h-6 w-6 items-center justify-center rounded-md hover:bg-rose-100/50 cursor-pointer"
          onClick={onClose}
          title="Dismiss message"
        >
          <X size={14} aria-hidden="true" />
        </button>
      ) : null}
    </div>
  );
}

export function SuccessMessage({ message, onClose }) {
  if (!message) {
    return null;
  }

  return (
    <div className="relative mb-4 rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 pr-10 text-sm font-semibold text-emerald-700 shadow-sm animate-fade-in">
      <p>{message}</p>
      {onClose ? (
        <button
          type="button"
          className="absolute right-2 top-2 flex h-6 w-6 items-center justify-center rounded-md hover:bg-emerald-100/50 cursor-pointer"
          onClick={onClose}
          title="Dismiss message"
        >
          <X size={14} aria-hidden="true" />
        </button>
      ) : null}
    </div>
  );
}

export function ToastMessage({ type = "success", message, onClose }) {
  if (!message) {
    return null;
  }

  const tone =
    type === "error"
      ? "border-rose-200 bg-rose-50 text-rose-700"
      : "border-emerald-200 bg-emerald-50 text-emerald-700";

  return (
    <div className={`fixed bottom-5 right-5 z-50 max-w-sm rounded-md border p-4 pr-11 text-sm font-semibold shadow-soft ${tone}`}>
      <p>{message}</p>
      {onClose ? (
        <button
          type="button"
          className="focus-ring absolute right-2 top-2 flex h-7 w-7 items-center justify-center rounded-md text-slate-500 hover:bg-white"
          onClick={onClose}
          title="Dismiss message"
        >
          <X size={15} aria-hidden="true" />
        </button>
      ) : null}
    </div>
  );
}

export function EmptyState({ title, description }) {
  return (
    <div className="rounded-md border border-dashed border-slate-300 bg-white p-8 text-center">
      <p className="text-base font-bold text-slate-900">{title}</p>
      {description ? <p className="mt-1 text-sm text-slate-500">{description}</p> : null}
    </div>
  );
}
