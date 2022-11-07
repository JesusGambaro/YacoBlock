import React from 'react'

const Amount = ({ transactions, type }) => {
  const uniqueTransactions = transactions.reduce((acc, transaction) => {
    const found = acc.find(
      (t) => t.sender.address === transaction.sender.address,
    )
    if (!found) {
      return [...acc, transaction]
    }
    return acc
  }, [])

  //amount debe
  const transactionsBySender = uniqueTransactions.filter(
    (tr) =>
      tr.sender.tipo == 'activo' ||
      tr.sender.tipo == 'pasivo' ||
      tr.sender.tipo == 'resultado positivo' ||
      tr.sender.tipo == 'resultado negativo' ||
      tr.sender.tipo == 'neto',
  )
  const transactionsByReceiver = uniqueTransactions.filter(
    (tr) =>
      tr.receiver.tipo == 'activo' ||
      tr.receiver.tipo == 'pasivo' ||
      tr.receiver.tipo == 'resultado positivo' ||
      tr.receiver.tipo == 'resultado negativo' ||
      tr.receiver.tipo == 'neto',
  )
  return (
    <div className="concepto-wrapper amount">
      {transactionsByReceiver.map((transaction, i) => {
        return (
          <p key={'transa ' + i} className="left-item">
            {transaction.amount}
          </p>
        )
      })}
      {transactionsBySender.map((transaction, i) => {
        return (
          <p key={'transa ' + i} className="right-item">
            {transaction.amount}
          </p>
        )
      })}
    </div>
  )
}

export default Amount
