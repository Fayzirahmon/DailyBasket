export default function Currency({ value }) {
  return <span>RM {Number(value ?? 0).toFixed(2)}</span>;
}
