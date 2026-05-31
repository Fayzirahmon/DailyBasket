import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";

export default function RevenueOverTimeLineChart({ orders }) {
  if (!orders || orders.length === 0) {
    return (
      <div className="p-4 text-center text-slate-500">
        No order data available
      </div>
    );
  }

  // Aggregate total revenue per day
  const grouped = orders.reduce((acc, order) => {
    // orderDate is like "2026-05-08T17:55:00"
    const date = new Date(order.orderDate);
    const day = date.toLocaleDateString("en-US", {
      month: "short",
      day: "numeric",
    }); // e.g. "May 08"

    acc[day] = (acc[day] || 0) + order.totalAmount;
    return acc;
  }, {});

  // Convert to array and sort by date
  // (Assuming data is mostly chronological, but I should sort just in case by turning 'day' back to timestamp if needed,
  // though for simple display, sorting by the original order array usually works if it's already sorted by date descending)
  // Sort by date
  const chartData = Object.keys(grouped)
    .sort(
      (a, b) =>
        new Date(`${a} ${new Date().getFullYear()}`) -
        new Date(`${b} ${new Date().getFullYear()}`),
    )
    .map((day) => ({
      day,
      revenue: grouped[day],
    }));

  return (
    <div className="rounded-md border border-slate-200 bg-white p-5 shadow-sm">
      <h2 className="text-lg font-bold text-slate-800 mb-4">
        Revenue Over Time
      </h2>
      <ResponsiveContainer width="100%" height={300}>
        <LineChart
          data={chartData}
          margin={{ top: 10, right: 10, left: -20, bottom: 0 }}
        >
          <CartesianGrid
            strokeDasharray="3 3"
            vertical={false}
            stroke="#E2E8F0"
          />
          <XAxis
            dataKey="day"
            axisLine={false}
            tickLine={false}
            tick={{ fill: "#64748b", fontSize: 12 }}
          />
          <YAxis
            axisLine={false}
            tickLine={false}
            tick={{ fill: "#64748b", fontSize: 12 }}
          />
          <Tooltip
            cursor={{
              stroke: "#cbd5e1",
              strokeWidth: 1,
              strokeDasharray: "3 3",
            }}
            contentStyle={{
              borderRadius: "8px",
              border: "none",
              boxShadow: "0 4px 6px -1px rgb(0 0 0 / 0.1)",
            }}
          />
          <Legend />
          <Line
            type="monotone"
            dataKey="revenue"
            name="Revenue (RM)"
            stroke="#059669"
            strokeWidth={3}
            dot={{ r: 4, fill: "#059669", strokeWidth: 0 }}
            activeDot={{ r: 6 }}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}
