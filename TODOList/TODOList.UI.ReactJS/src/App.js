import { useState, useEffect } from 'react';

import logo from './logo.svg';
import './App.css';

import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.js";

function App() {
    const [todos, setTodo] = useState([]);

    useEffect(() => {
        var url = "https://localhost:7239/Todo";

        var fetchData = async () => {
            // You can await here
            var response = await fetch(url, {
                mode: 'cors',
                method: "get",
                ContentType: 'application/json'
            })

            var json = await response.json();

            setTodo(json);
        }

        fetchData();
    }, []);

    return (
      <div className="App">
          <div className="container">
            <nav className="navbar navbar-light">
                <div className="navbar-brand" href="#">
                  <div className="d-inline-block align-top">📝</div>
                  <p>TODO list</p>
                </div> 
            </nav>
            <table className="table">
                <thead>
                    <tr className="bg-dark text-light">
                        <th scope="col">ID</th>
                        <th scope="col">Description</th>
                        <th scope="col">Completed Yes ✅ / No ❌</th>
                        <th scope="col"> Edit 📝 / Remove ❌</th>
                    </tr>
                </thead>

                <tbody>
                        {todos.map(item => (
                            <tr>
                                <th scope="row">{ item.id }</th>
                                <td> { item.name } </td>
                                <td> {item.isCompleted ? "Yes ✅" : "No ❌"} </td>
                                <td scope="col">
                                    <a href="#">Edit</a>📝 / <a href="#">Remove</a> ❌
                                </td>
                            </tr>  
                        ))}
                </tbody>
            </table>
        </div>
    </div>
  );
}

export default App;
