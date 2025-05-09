import { useState, useEffect } from "react";
import axios from "../api/axios";
import type { Product } from "../types/Product";
import { toast } from "react-toastify";

interface Props {
  current: Product | null;
  refresh: () => void;
}

const ProductForm = ({ current, refresh }: Props) => {
  const [form, setForm] = useState<Omit<Product, "id" | "createdAt">>({
    name: "",
    description: "",
    price: 0,
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (current) {
      setForm({
        name: current.name,
        description: current.description,
        price: current.price,
      });
    }
  }, [current]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      if (current) {
        await axios.put(`/product/${current.id}`, form);
      } else {
        await axios.post("/product", form);
      }
      setForm({ name: "", description: "", price: 0 });
      toast.success(current ? 'Product updated successfully!' : 'Product added successfully!');
      refresh();
    } catch {
        toast.error('Failed to save product.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="border p-4 rounded shadow-sm bg-light"
    >
      <h5 className="mb-3">{current ? "✏️ Edit Product" : "➕ Add Product"}</h5>
      <div className="mb-2">
        <label className="form-label">Name</label>
        <input
          className="form-control"
          value={form.name}
          onChange={(e) => setForm({ ...form, name: e.target.value })}
        />
      </div>
      <div className="mb-2">
        <label className="form-label">Description</label>
        <textarea
          className="form-control"
          value={form.description}
          onChange={(e) => setForm({ ...form, description: e.target.value })}
        ></textarea>
      </div>
      <div className="mb-3">
        <label className="form-label">Price</label>
        <input
          type="number"
          className="form-control"
          value={form.price}
          onChange={(e) => setForm({ ...form, price: +e.target.value })}
        />
      </div>
      <button className="btn btn-success w-100" disabled={isSubmitting}>
        {isSubmitting ? "Saving..." : current ? "Update" : "Create"}
      </button>
    </form>
  );
};

export default ProductForm;
