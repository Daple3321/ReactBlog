import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import { AuthProvider } from "react-oidc-context";

const oidcConfig = {
    authority: "http://localhost:8080/realms/blog-platform",
    client_id: "blog-spa",
    redirect_uri: window.location.origin,
    post_logout_redirect_uri: window.location.origin,
    response_type: "code",
    scope: "openid profile email",
    automaticSilentRenew: true,
    onSigninCallback: () => window.history.replaceState({}, document.title, window.location.pathname),
};

createRoot(document.getElementById('root')).render(
  <StrictMode>
      <AuthProvider {...oidcConfig}>
        <App />
      </AuthProvider>
  </StrictMode>,
)
