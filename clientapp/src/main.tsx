import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { Provider } from "react-redux";
import { AppContext, appServices } from "./app/common/services/appservices.ts";
import storeKeeper from "./store/storeKeeper.ts";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <Provider store={storeKeeper}>
      <AppContext.Provider value={appServices}>
        <App />
      </AppContext.Provider>
    </Provider>
  </StrictMode>
);
