import React from 'react'
import './loader.scss'
const Loader = () => {
  return (
    <div className="spinner">
      {Array(6)
        .fill()
        .map((_, i) => (
          <div key={'loader ' + i} />
        ))}
    </div>
  )
}

export default Loader
