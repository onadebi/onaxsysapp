import { HelmetProvider } from "react-helmet-async";
import { RouterProvider } from "react-router-dom";
import "./App.css";
import AppRouter from "./routes/AppRouter";
import MetaTag from "./app/_components/MetaTag";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import Loader from "./app/_components/Loader";

function App() {
  const queryClient = new QueryClient();
  return (
    <>
      <HelmetProvider>
        <QueryClientProvider client={queryClient}>
          <Loader/>
          <MetaTag title="Onadebi" />
          <RouterProvider router={AppRouter} />
          <ToastContainer position="bottom-right" />
          </QueryClientProvider>
      </HelmetProvider>
    </>
  );
}

export default App;
