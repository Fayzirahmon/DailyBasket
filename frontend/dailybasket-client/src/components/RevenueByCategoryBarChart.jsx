import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid,
  Tooltip, Legend, ResponsiveContainer
} from "recharts";

export default function RevenueByCategoryBarChart({ orders, products }) {
  if (!orders || !products || orders.length === 0) {
    return <div className="p-4 text-center text-slate-500">No data available</div>;
  }

  // Calculate revenue per category
  const revenueByCategory = {};

  orders.forEach(order => {
    order.items?.forEach(item => {
      const product = products.find(p => p.productId === item.productId);
      const category = product ? product.categoryName : "Unknown";
      revenueByCategory[category] = (revenueByCategory[category] || 0) + item.lineTotal;
    });
  });

  const chartData = Object.entries(revenueByCategory).map(([category, revenue]) => ({
    category,
    revenue: Number(revenue.toFixed(2))
  }));

  return (
    <div className="rounded-md border border-slate-200 bg-white p-5 shadow-sm">
      <h2 className="text-lg font-bold text-slate-800 mb-4">Revenue by Category</h2>
      <ResponsiveContainer width="100%" height={300}>
        <BarChart data={chartData} margin={{ top: 10, right: 10, left: -20, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#E2E8F0" />
          <XAxis dataKey="category" axisLine={false} tickLine={false} tick={{ fill: '#64748b', fontSize: 12 }} />
          <YAxis axisLine={false} tickLine={false} tick={{ fill: '#64748b', fontSize: 12 }} />
          <Tooltip 
            cursor={{ fill: '#f1f5f9' }}
            contentStyle={{ borderRadius: '8px', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
          />
          <Legend />
          <Bar dataKey="revenue" name="Revenue (RM)" fill="#0369a1" radius={[4, 4, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
