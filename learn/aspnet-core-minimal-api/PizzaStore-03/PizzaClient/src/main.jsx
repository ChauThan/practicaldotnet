import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import Pizza from './Pizza'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <Pizza />
  </StrictMode>,
)
