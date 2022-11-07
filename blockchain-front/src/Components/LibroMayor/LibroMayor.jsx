import { useState, useEffect } from 'react'
import './libroMayor.scss'
import axios from 'axios'
import Loader from '../Loader/Loader'
const LibroMayor = () => {
  const [libroMayor, setLibroMayor] = useState([])
  const [loading, setLoading] = useState(true)
  useEffect(() => {
    axios
      .get('https://localhost:9000/Transactions/GetLibrosMayores')
      .then((res) => {
        setLibroMayor(res.data)
      })
      .finally(() => {
        setLoading(false)
      })
  }, [])

  return (
    <main>
      <header>
        <h1>Libro Mayor</h1>
      </header>
      {loading ? (
        <Loader />
      ) : (
        <div className="libro-mayor-container">
          {libroMayor.map((tr, i) => {
            return (
              <article key={i} className="t-container">
                <div className="t-header">
                  <p>Debe</p>
                  <h1>{tr.cuenta}</h1>
                  <p>Haber</p>
                </div>
                <div className="t-body">
                  <div className="count-wrapper">
                    <ul className="debe-list">
                      {tr.debeList.map((d, id) => {
                        return <li key={id}>{d}</li>
                      })}
                      {tr.debeList.length > 0 && (
                        <li>
                          <h2>Total: {tr.debe}</h2>
                        </li>
                      )}
                    </ul>
                    <ul className="haber-list">
                      {tr.haberList.map((d, id) => {
                        return <li key={id}>{d}</li>
                      })}
                      {tr.haberList.length > 0 && (
                        <li>
                          <h2>Total: {tr.haber}</h2>
                        </li>
                      )}
                    </ul>
                  </div>

                  <div className="t-footer">
                    <h2>
                      {tr.saldo < 0 ? 'Saldo acreedor' : 'Saldo deudor'}:{' '}
                      {Math.abs(tr.saldo)}
                    </h2>
                  </div>
                </div>
              </article>
            )
          })}
        </div>
      )}
    </main>
  )
}

export default LibroMayor
