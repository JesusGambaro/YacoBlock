import React from 'react'
import Loader from '../Loader/Loader'
const Home = () => {
  return (
    <main className="home">
      <h1 style={{ marginBottom: '2rem' }}>YacoChain</h1>
      <Loader />
      <h2 style={{ marginTop: '2rem' }}>Home page</h2>
    </main>
  )
}

export default Home
