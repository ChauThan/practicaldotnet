import { useEffect, useState } from "react";
import PizzaList from "./PizzaList";

const term = "Pizza";

function Pizza() {
    const [data, setData] = useState([]);
    const [maxId, setMaxId] = useState(0);

    useEffect(() => {
        fetchPizzaData();
    }, []);

    const fetchPizzaData = () => {
        // Simulate an API call
        const pizzaData = [
            { id: 1, name: 'Margherita', description: 'Tomato sauce, mozzarella, and basil' },
            { id: 2, name: 'Pepperoni', description: 'Tomato sauce, mozzarella, and pepperoni' },
            { id: 3, name: 'Hawaiian', description: 'Tomato sauce, mozzarella, ham, and pineapple' },
        ];

        setData(pizzaData);
        setMaxId(Math.max(...pizzaData.map(pizza => pizza.id)));
    };

    const handleCreate = (item) => {
        const newPizza = { ...item, id: maxId + 1 };
        setData([...data, newPizza]);
        setMaxId(maxId + 1);
    };

    const handleUpdate = (item) => {
        const updatedData = data.map(pizza => pizza.id === item.id ? item : pizza);
        setData(updatedData);
    };

    const handleDelete = (id) => {
        const updatedData = data.filter(pizza => pizza.id !== id);
        setData(updatedData);
    };

    return (
        <div>
            <PizzaList
                name={term}
                data={data}
                onCreate={handleCreate}
                onUpdate={handleUpdate}
                onDelete={handleDelete}
            />
        </div>
    )
}

export default Pizza;