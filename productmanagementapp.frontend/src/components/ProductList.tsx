import { useEffect, useState } from "react";
import axios from "../api/axios";
import type { Product } from "../types/Product";
import { formatRupiah } from "../utils/format";
import { toast } from "react-toastify";

interface Props {
  onEdit: (product: Product) => void;
  reload: boolean;
}

const ProductList = ({ onEdit, reload }: Props) => {
  const [products, setProducts] = useState<Product[]>([]);
  const [name, setName] = useState("");
  const [minPrice, setMinPrice] = useState("");
  const [maxPrice, setMaxPrice] = useState("");

  const load = async () => {
    const params: Record<string, string> = {};
    if (name) params.name = name;
    if (minPrice) params.minPrice = minPrice;
    if (maxPrice) params.maxPrice = maxPrice;

    const res = await axios.get<Product[]>("/product", { params });
    setProducts(res.data);
  };

  const handleDelete = async (id: number) => {
    if (window.confirm("Are you sure to delete this product?")) {
      try {
        await axios.delete(`/product/${id}`);
        toast.success("Product deleted successfully!");
        load();
      } catch {
        toast.error("Failed to delete product.");
      }
    }
  };

  const handleReset = () => {
    setName("");
    setMinPrice("");
    setMaxPrice("");
  };

  //search/filtering
  useEffect(() => {
    const delayDebounce = setTimeout(() => {
      load();
    }, 500); // debounce 500ms

    return () => clearTimeout(delayDebounce);
  }, [name, minPrice, maxPrice]);

  // Trigger ulang jika ada perubahan dari luar (ex: after edit/create)
  useEffect(() => {
    load();
  }, [reload]);

  return (
    <div className="shadow-sm p-3 bg-white rounded">
      <h5 className="mb-3">📦 Product List</h5>

      <div className="row g-2 mb-3">
        <div className="col-md-4">
          <input
            className="form-control"
            placeholder="Search by name"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
        <div className="col-md-3">
          <input
            className="form-control"
            type="number"
            placeholder="Min Price"
            value={minPrice}
            onChange={(e) => setMinPrice(e.target.value)}
          />
        </div>
        <div className="col-md-3">
          <input
            className="form-control"
            type="number"
            placeholder="Max Price"
            value={maxPrice}
            onChange={(e) => setMaxPrice(e.target.value)}
          />
        </div>
        <div className="col-md-2">
          <button
            className="btn btn-outline-secondary w-100"
            onClick={handleReset}
          >
            Reset
          </button>
        </div>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>Name</th>
            <th>Price</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {products.map((p) => (
            <tr key={p.id}>
              <td>{p.name}</td>
              <td>{formatRupiah(p.price)}</td>
              <td>
                <button
                  className="btn btn-sm btn-warning me-2"
                  onClick={() => onEdit(p)}
                >
                  Edit
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(p.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
          {products.length === 0 && (
            <tr>
              <td colSpan={3} className="text-center">
                No products found.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default ProductList;
