import {
  ShoppingBasket,
  ClipboardList,
  LayoutDashboard,
  Package,
  ShoppingCart,
  Tags,
  Users,
  Shield,
  User,
  LogOut
} from "lucide-react";

const navItems = [
  { key: "dashboard", label: "Dashboard", icon: LayoutDashboard },
  { key: "products", label: "Products", icon: Package },
  { key: "categories", label: "Categories", icon: Tags },
  { key: "customers", label: "Customers", icon: Users },
  { key: "cart", label: "Cart", icon: ShoppingCart },
  { key: "orders", label: "Orders", icon: ClipboardList }
];

export default function AppLayout({
  activePage,
  onNavigate,
  customers,
  activeCustomerId,
  onCustomerChange,
  session,
  onLogout,
  children
}) {
  const filteredNavItems = navItems.filter((item) => {
    if (session?.role === "admin") {
      return item.key !== "cart";
    } else {
      return ["products", "cart", "orders"].includes(item.key);
    }
  });

  return (
    <div className="min-h-screen bg-slate-100 text-slate-900">
      <aside className="fixed inset-y-0 left-0 z-20 hidden w-64 border-r border-slate-200 bg-white lg:block">
        <div className="flex h-16 items-center gap-3 border-b border-slate-200 px-5">
          <div className="flex h-10 w-10 items-center justify-center rounded-md bg-emerald-600 text-white">
            <ShoppingBasket size={22} aria-hidden="true" />
          </div>
          <div>
            <p className="text-lg font-bold">DailyBasket</p>
            <p className="text-xs font-medium text-slate-500">Grocery System</p>
          </div>
        </div>

        <nav className="space-y-1 px-3 py-4">
          {filteredNavItems.map((item) => {
            const Icon = item.icon;
            const isActive = activePage === item.key;
            return (
              <button
                key={item.key}
                type="button"
                onClick={() => onNavigate(item.key)}
                className={`focus-ring flex w-full items-center gap-3 rounded-md px-3 py-2 text-left text-sm font-semibold transition ${
                  isActive
                    ? "bg-emerald-50 text-emerald-700"
                    : "text-slate-600 hover:bg-slate-100 hover:text-slate-950"
                }`}
              >
                <Icon size={18} aria-hidden="true" />
                {item.label}
              </button>
            );
          })}
        </nav>
      </aside>

      <div className="lg:pl-64">
        <header className="sticky top-0 z-10 border-b border-slate-200 bg-white/95 backdrop-blur">
          <div className="flex min-h-16 flex-col gap-3 px-4 py-3 sm:flex-row sm:items-center sm:justify-between lg:px-8">
            <div className="flex items-center gap-3 lg:hidden">
              <div className="flex h-9 w-9 items-center justify-center rounded-md bg-emerald-600 text-white">
                <ShoppingBasket size={20} aria-hidden="true" />
              </div>
              <span className="font-bold">DailyBasket</span>
            </div>

            <nav className="flex gap-2 overflow-x-auto lg:hidden">
              {filteredNavItems.map((item) => {
                const Icon = item.icon;
                return (
                  <button
                    key={item.key}
                    type="button"
                    title={item.label}
                    onClick={() => onNavigate(item.key)}
                    className={`focus-ring flex h-10 min-w-10 items-center justify-center rounded-md border text-sm ${
                      activePage === item.key
                        ? "border-emerald-200 bg-emerald-50 text-emerald-700"
                        : "border-slate-200 bg-white text-slate-600"
                    }`}
                  >
                    <Icon size={18} aria-hidden="true" />
                  </button>
                );
              })}
            </nav>

            <div className="flex items-center justify-between gap-4 sm:ml-auto w-full sm:w-auto">
              <div className="flex items-center gap-2 rounded-lg bg-slate-50 border border-slate-200 px-3 py-1.5 shadow-sm">
                {session?.role === "admin" ? (
                  <>
                    <Shield size={16} className="text-emerald-600" />
                    <span className="text-xs font-bold text-slate-800">Admin Portal</span>
                  </>
                ) : (
                  <>
                    <User size={16} className="text-blue-600" />
                    <span className="text-xs font-bold text-slate-800">
                      {session?.customer?.fullName || "Customer"}
                    </span>
                  </>
                )}
              </div>

              <button
                type="button"
                onClick={onLogout}
                title="Log out"
                className="focus-ring inline-flex h-9 w-9 items-center justify-center rounded-md border border-slate-200 bg-white text-slate-600 hover:bg-slate-50 hover:text-slate-900 cursor-pointer"
              >
                <LogOut size={16} aria-hidden="true" />
              </button>
            </div>
          </div>
        </header>

        <main className="px-4 py-6 sm:px-6 lg:px-8">{children}</main>
      </div>
    </div>
  );
}
