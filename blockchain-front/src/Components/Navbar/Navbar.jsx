import React from 'react'
import './navbar.scss'
import { useNavigate } from 'react-router-dom'

const Navbar = () => {
  const navigate = useNavigate()
  return (
    //re puto el de la navbar
    <nav>
      <span className="logo">
        <h1 onClick={() => navigate('/')} className="logo-name">
          YacoChain>
        </h1>
      </span>

      <div className="nav-wrapper">
        <ul className="nav-menu">
          <li className="nav-item">
            <p className="navLink" onClick={() => navigate('/LibroDiario')}>
              Libro Diario
            </p>
          </li>
          <li className="nav-item">
            <p className="navLink" onClick={() => navigate('/LibroMayor')}>
              Libro Mayor
            </p>
          </li>
        </ul>
      </div>
    </nav>
  )
}

export default Navbar
