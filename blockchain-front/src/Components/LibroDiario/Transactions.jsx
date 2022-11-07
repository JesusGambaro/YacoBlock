import axios from 'axios'
import React from 'react'

const Transactions = ({ transactions, editable, updateState }) => {
  const deleteTransaction = (id) => {
    axios
      .delete(`https://localhost:9000/Transactions/DeleteTransaction?id=${id}`)
      .then((transactions = transactions.filter((t) => t.id !== id)))
      .catch()
      .finally(updateState())
  }

  return (
    <div className="concepto-wrapper">
      {transactions.map((transaction, i) => {
        return (
          <p key={'transa ' + i} className="left-item">
            {transaction.sender.address}
            {editable && (
              <i
                className="fa-solid fa-trash-can"
                title="Delete?"
                onClick={() => deleteTransaction(transaction.id)}
              ></i>
            )}
          </p>
        )
      })}
      {transactions.map((transaction, i) => {
        return (
          <p key={'transa ' + i} className="right-item">
            a {transaction.receiver.address}
          </p>
        )
      })}
    </div>
  )
}

export default Transactions
