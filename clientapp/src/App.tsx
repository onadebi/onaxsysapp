import { HelmetProvider } from "react-helmet-async";
import { RouterProvider } from "react-router-dom";
import {ClerkProvider} from "@clerk/clerk-react";
import "./App.css";
import AppRouter from "./routes/AppRouter";
import MetaTag from "./app/_components/MetaTag";
import appsettings from "./app/common/config/appsettings";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import Loader from "./app/_components/Loader";
import BlogAppRoutes from "./app/blogapp/BlogAppRoutes";

function App() {
  const queryClient = new QueryClient();
  return (
    <>
      <HelmetProvider>
        <ClerkProvider publishableKey={appsettings.Auth.Clerk.PUBLISHABLE_KEY} 
        afterSignOutUrl={`${BlogAppRoutes().homebase}${BlogAppRoutes().public.home.parentRoute}`}
        >
        <QueryClientProvider client={queryClient}>
          <Loader/>
          <MetaTag title="Onadebi" />
          <RouterProvider router={AppRouter} />
          <ToastContainer position="bottom-right" />
          </QueryClientProvider>
        </ClerkProvider>
      </HelmetProvider>
    </>
  );
}

export default App;
