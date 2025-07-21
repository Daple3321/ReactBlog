import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import NavBar from './components/NavBar.jsx'
import BlogButton from './components/BlogButton.jsx'
import BlogView from './components/BlogView.jsx'
import { BlogProvider } from './blogTools.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
        <App />
  </StrictMode>,
)
