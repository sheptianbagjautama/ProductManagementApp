import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "./components/Login";
import Register from "./components/Register";
import PrivateRoute from "./components/PrivateRoute";
import { useState } from "react";
import ProductForm from "./components/ProductForm";
import ProductList from "./components/ProductList";
import type { Product } from "./types/Product";

function Home() {
  const [selected, setSelected] = useState<Product | null>(null);

  const [reload, setReload] = useState(false);

  const refresh = () => {
    setSelected(null);
    setReload((prev) => !prev);
  };

  const logout = () => {
    localStorage.removeItem("token");
    window.location.href = "/login";
  };

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>🛍️ Product Management</h2>
        <button className="btn btn-outline-danger btn-sm" onClick={logout}>
          Logout
        </button>
      </div>
      <div className="row">
        <div className="col-md-5">
          <ProductForm current={selected} refresh={refresh} />
        </div>
        <div className="col-md-7">
          <ProductList onEdit={setSelected} reload={reload} />
        </div>
      </div>
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route
          path="/"
          element={
            <PrivateRoute>
              <Home />
            </PrivateRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
