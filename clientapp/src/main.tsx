import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { Provider } from "react-redux";
import { AppContext, appServices } from "./app/common/services/appservices.ts";
import storeKeeper from "./store/storeKeeper.ts";
import { GoogleOAuthProvider } from '@react-oauth/google';
import appsettings from "./app/common/config/appsettings.ts";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <Provider store={storeKeeper}>
      <AppContext.Provider value={appServices}>
      <GoogleOAuthProvider clientId={appsettings.externalServices.GoogleAuth.clientId}>
        <App />
      </GoogleOAuthProvider>
      </AppContext.Provider>
    </Provider>
  </StrictMode>
);
