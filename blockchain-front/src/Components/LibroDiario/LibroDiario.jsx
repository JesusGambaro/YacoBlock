import { useState, useEffect } from "react";
import "./libroDiario.scss";
import axios from "axios";
import Transactions from "./Transactions";
import Amount from "./Amount";
import Loader from "../Loader/Loader";
import ConfirmDialog from "../ConfirmDialog/ConfirmDialog";
import DatePicker from "../DatePicker/DatePicker";
const TRANSACTION_MODEL = {
  sender: {
    address: "",
    tipo: "activo",
  },
  receiver: {
    address: "",
    tipo: "pasivo",
  },
  amount: 0,
};
const LibroDiario = () => {
  const [libroDiario, setLibroDiario] = useState([]);
  const [pendingTransactions, setPendingTransactions] = useState([]);
  const [loading, setLoading] = useState(false);
  const [dialogActive, setDialogActive] = useState(false);
  const [dateActive, setDateActive] = useState(false);
  const [totalesLibroDiario, setTotalesLibroDiario] = useState({
    haberes: 0,
    deudores: 0,
  });

  const [error, setError] = useState({
    sender: {
      address: "",
      tipo: "",
    },
    receiver: {
      address: "",
      tipo: "",
    },
    amount: "",
  });
  const [newTransaction, setNewTransaction] = useState({
    sender: {
      address: "",
      tipo: "activo",
    },
    receiver: {
      address: "",
      tipo: "pasivo",
    },
    amount: 0,
  });
  useEffect(() => {
    setLoading(true);
    axios
      .get("https://localhost:9000/Blockchain")
      .then((res) => {
        setLibroDiario(res.data.chain);
        setPendingTransactions(res.data.pendingTransactions);
      })
      .finally(() => {
        
        calculateTotales();
      });
  }, []);

  const calculateTotales = () => {
    axios.get("https://localhost:9000/Blockchain/GetTotales").then((res) => {
      console.log("Totales");
      console.log(res);
      setTotalesLibroDiario({
        haberes: res.data[0],
        deudores: res.data[1],
      });
    }).finally(() => {
      setLoading(false);
    });
  };
  const parseDate = (date) => {
    const d = new Date(date);
    return `${
      d.getDate().toString().length == 1
        ? "0" + d.getDate().toString()
        : d.getDate()
    }/${d.getMonth() + 1}/${d.getFullYear()}`;
  };

  const verifyErrors = () => {
    let hasErrors = false;
    let newError = error;
    if (
      !newTransaction.sender.address.length ||
      newTransaction.sender.address == ""
    ) {
      hasErrors = true;
      newError = {
        ...newError,
        sender: { ...newError.sender, address: "Este campo es requerido." },
      };
    } else {
      newError = {
        ...newError,
        sender: { ...newError.sender, address: "" },
      };
    }
    if (
      !newTransaction.sender.tipo.length ||
      newTransaction.sender.tipo == ""
    ) {
      hasErrors = true;
      newError = {
        ...newError,
        sender: { ...newError.sender, tipo: "Este campo es requerido." },
      };
    } else {
      newError = {
        ...newError,
        sender: { ...newError.sender, tipo: "" },
      };
    }
    if (
      !newTransaction.receiver.address.length ||
      newTransaction.receiver.address == ""
    ) {
      hasErrors = true;
      newError = {
        ...newError,
        receiver: { ...newError.receiver, address: "Este campo es requerido." },
      };
    } else {
      newError = {
        ...newError,
        receiver: { ...newError.receiver, address: "" },
      };
    }
    if (
      !newTransaction.receiver.tipo.length ||
      newTransaction.receiver.tipo == ""
    ) {
      hasErrors = true;
      newError = {
        ...newError,
        receiver: { ...newError.receiver, tipo: "Este campo es requerido." },
      };
    } else {
      newError = {
        ...newError,
        receiver: { ...newError.receiver, tipo: "" },
      };
    }
    if (newTransaction.amount <= 0) {
      hasErrors = true;
      newError = {
        ...newError,
        amount: "Este campo es requerido.",
      };
    } else {
      newError = {
        ...newError,
        amount: "",
      };
    }
    setError(newError);
    return hasErrors;
  };

  const addTransaction = (e) => {
    setLoading(true);
    e.preventDefault();
    if (verifyErrors()) {
      setLoading(false);
      return;
    }
    console.log("Sending transaction");
    let transaction = newTransaction;
    transaction.sender.address =
      transaction.sender.address.substring(0, 1).toUpperCase() +
      transaction.sender.address.substring(1).toLowerCase();

    transaction.receiver.address =
      transaction.receiver.address.substring(0, 1).toUpperCase() +
      transaction.receiver.address.substring(1).toLowerCase();
    transaction.sender.tipo = transaction.sender.tipo.toLowerCase();
    transaction.receiver.tipo = transaction.receiver.tipo.toLowerCase();
    axios
      .post(
        "https://localhost:9000/Transactions/AddTransaction",
        newTransaction
      )
      .then()
      .catch((err) => {
        console.error(err);
      })
      .finally(() => {
        axios
          .get("https://localhost:9000/Blockchain")
          .then((res) => {
            setLibroDiario(res.data.chain);
            setPendingTransactions(res.data.pendingTransactions);
          })
          .finally(() => {
            setLoading(false);
            calculateTotales();
          });
        setNewTransaction(TRANSACTION_MODEL);
      });
  };
  const handleSubmitTransactions = (date) => {
    setLoading(true);
    const config = { headers: {'Content-Type': 'application/json'} };
    axios
      .post("https://localhost:9000/Blockchain/mine",date,config)
      .then()
      .catch()
      .finally(() => {
        axios
          .get("https://localhost:9000/Blockchain")
          .then((res) => {
            setLibroDiario(res.data.chain);
            setPendingTransactions(res.data.pendingTransactions);
          })
          .finally(() => {
            setLoading(false);
            calculateTotales();
          });
      });
  };
  useEffect(() => {}, [loading]);
  const updateState = (e) => {
    setLoading(true);
    axios
      .get("https://localhost:9000/Blockchain")
      .then((res) => {
        setLibroDiario(res.data.chain);
        setPendingTransactions(res.data.pendingTransactions);
      })
      .finally(() => {
        setLoading(false);
        calculateTotales();
      });
  };
  const keyDown = (e) => {
    if (e.ctrlKey && e.key == "r") {
      updateState();
    } else if (e.key === "Enter" && e.ctrlKey) {
      handleSubmitTransactions(e);
    } else if (e.key === "Enter") {
      addTransaction(e);
    }
  };
  const deleteBlockchain = () => {
    setLoading(true);
    axios
      .delete("https://localhost:9000/Blockchain")
      .then((res) => {
        //console.log(res);
      })
      .catch((res) => {
        //console.log(res);
      })
      .finally(() => {
        axios
          .get("https://localhost:9000/Blockchain")
          .then((res) => {
            setLibroDiario(res.data.chain);
            setPendingTransactions(res.data.pendingTransactions);
          })
          .finally(() => {
        
            calculateTotales();
          });
      });
    setNewTransaction(TRANSACTION_MODEL);
  };
  document.onkeydown = keyDown;
  return (
    <main>
      <header>
        <h1>Libro Diario</h1>
        {!loading && (
          <button
            onClick={() => {
              setDialogActive(!dialogActive);
            }}
          >
            Delete Blockchain
          </button>
        )}
      </header>
      {loading ? (
        <Loader />
      ) : (
        <div className="libro-table">
          {/*Table header*/}
          <div className="libro-table__header">
            <div className="libro-table__header__item">Fecha</div>
            <div className="libro-table__header__item">Concepto</div>
            <div className="libro-table__header__item">Debe</div>
            <div className="libro-table__header__item">Haber</div>
          </div>
          <div className="libro-table__body">
            {/*Table body
          Show all transactions of each block
          */}
            {libroDiario.map((block, i) => {
              if (i == 0) return null;
              return (
                <div className="libro-table__body__row" key={"block " + i}>
                  {/*Table row*/}
                  <div className="libro-table__body__row__item date">
                    {/*Table row item*/}
                    {parseDate(block.timeStamp)}
                  </div>
                  <div className="libro-table__body__row__item">
                    <Transactions transactions={block.transactions} />
                  </div>
                  <div className="libro-table__body__row__item amount">
                    <Amount transactions={block.transactions} />
                  </div>
                </div>
              );
            })}
            {/*Show pending transation
             *In progress of being added to the blockchain
             */}
            {pendingTransactions.length ? (
              <div className="libro-table__body__row">
                <div className="libro-table__body__row__item date">
                  <button
                    className="register-btn"
                    //onClick={handleSubmitTransactions}
                    onClick={() => {
                      setDateActive(true)
                    } }
                  >
                    Registrar
                  </button>
                </div>
                <div className="libro-table__body__row__item">
                  <Transactions
                    transactions={pendingTransactions}
                    editable={true}
                    updateState={updateState}
                  />
                </div>
                <div className="libro-table__body__row__item amount">
                  <Amount transactions={pendingTransactions} />
                </div>
              </div>
            ) : null}
            {/*Add new transaction*/}
            <div className="libro-table__body__row add">
              <div className="libro-table__body__row__item">
                <button className="add-btn" onClick={addTransaction}>
                  Agregar
                </button>
              </div>
              <div className="libro-table__body__row__item">
                <div className="add-wrapper">
                  <p>Agregar Sender</p>
                  <input
                    type="text"
                    onChange={(e) =>
                      setNewTransaction({
                        ...newTransaction,
                        sender: {
                          ...newTransaction.sender,
                          address: e.target.value,
                        },
                      })
                    }
                  />
                  <p style={{ color: "red" }}>{error.sender.address}</p>
                  <p>Tipo</p>
                  <select
                    name="tipo"
                    id="tipo"
                    onChange={(e) =>
                      setNewTransaction({
                        ...newTransaction,
                        receiver: {
                          ...newTransaction.receiver,
                          tipo: e.target.value,
                        },
                      })
                    }
                    defaultValue={"activo"}
                  >
                    <option value="activo">Activo</option>
                    <option value="pasivo">Pasivo</option>
                    <option value="neto">P.Neto</option>
                  </select>
                  <p style={{ color: "red" }}>{error.sender.tipo}</p>
                </div>
              </div>
              <div className="libro-table__body__row__item">
                <div className="add-wrapper">
                  <p>Agregar Receiver</p>
                  <input
                    type="text"
                    onChange={(e) =>
                      setNewTransaction({
                        ...newTransaction,
                        receiver: {
                          ...newTransaction.receiver,
                          address: e.target.value,
                        },
                      })
                    }
                  />
                  <p style={{ color: "red" }}>{error.receiver.address}</p>
                  <p>Tipo</p>
                  <select
                    name="tipo"
                    id="tipo"
                    onChange={(e) =>
                      setNewTransaction({
                        ...newTransaction,
                        receiver: {
                          ...newTransaction.receiver,
                          tipo: e.target.value,
                        },
                      })
                    }
                    defaultValue={"pasivo"}
                  >
                    <option value="activo">Activo</option>
                    <option value="pasivo">Pasivo</option>
                    <option value="resultado positivo">
                      Resultado positivo
                    </option>
                    <option value="resultado negativo">
                      Resultado negativo
                    </option>
                    <option value="neto">P.Neto</option>
                  </select>
                  <p style={{ color: "red" }}>{error.receiver.tipo}</p>
                </div>
              </div>
              <div className="libro-table__body__row__item add">
                <div className="add-wrapper">
                  <p>Agregar Cantidad</p>
                  <input
                    type="number"
                    onChange={(e) =>
                      setNewTransaction({
                        ...newTransaction,
                        amount: e.target.value,
                      })
                    }
                  />
                  <p style={{ color: "red" }}>{error.amount}</p>
                </div>
              </div>
            </div>
            <div className="libro-table__body__row total">
              <div className="libro-table__body__row__item">
                <h1 className="total-ld">Total: </h1>
              </div>
              <div className="libro-table__body__row__item">
                <div className="total-wrapper">
                  <p className="left-item">{totalesLibroDiario.deudores}</p>
                  <p className="rigth-item">{totalesLibroDiario.haberes}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
      {dateActive && (
        <DatePicker acceptFunc={(date) => {
          handleSubmitTransactions(date);
          setDateActive(false);
        }} cancelFunc={() => {
          setDateActive(false);
        }}></DatePicker>
      )}
      {dialogActive && (
        <ConfirmDialog
          cancelFunc={() => {
            setDialogActive(false);
          }}
          acceptFunc={() => {
            deleteBlockchain();
            setDialogActive(false);
          }}
        ></ConfirmDialog>
      )}
    </main>
  );
};

export default LibroDiario;
