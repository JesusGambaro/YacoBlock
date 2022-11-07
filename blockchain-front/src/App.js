import { BrowserRouter } from 'react-router-dom'
import LibroDiario from './Components/LibroDiario/LibroDiario'
import LibroMayor from './Components/LibroMayor/LibroMayor'
import { Routes, Route } from 'react-router-dom'
import Navbar from './Components/Navbar/Navbar'
import Home from './Components/Home/Home'

function App() {
  return (
    <div className="App">
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/LibroDiario" element={<LibroDiario />} />
        <Route path="/LibroMayor" element={<LibroMayor />} />
      </Routes>
    </div>
  )
}

export default App
