import { useState } from "react";

function PizzaList({ name, data, onCreate, onUpdate, onDelete, error }) {
    const [formData, setFormData] = useState({ id: '', name: '', description: '' });
    const [editingId, setEditingId] = useState(null);

    const handleFormChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    }

    const handleSubmit = (e) => {
        e.preventDefault();
        if (editingId) {
            onUpdate({ ...formData, id: editingId });
            setEditingId(null);
        } else {
            onCreate(formData);
        }
        setFormData({ id: '', name: '', description: '' });
    }

    const handleEdit = (pizza) => {
        setFormData({
            id: pizza.id,
            name: pizza.name,
            description: pizza.description
        });
        setEditingId(pizza.id);
    }

    const handleCancelEdit = () => {
        setFormData({ id: '', name: '', description: '' });
        setEditingId(null);
    }

    return (
        <div>
            <h2>New {name}</h2>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    name="name"
                    placeholder="Name"
                    value={formData.name}
                    onChange={handleFormChange}
                />
                <input
                    type="text"
                    name="description"
                    placeholder="Description"
                    value={formData.description}
                    onChange={handleFormChange}
                />
                <button type="submit">{editingId ? 'Update' : 'Create'}</button>
                {editingId && <button type="button" onClick={handleCancelEdit}>Cancel</button>}
            </form>
            {error && <div>{error.message}</div>}
            <h2>{name}s</h2>
            <ul>
                {data.map(pizza => (
                    <li key={pizza.id}>
                        <div><strong>{pizza.name}</strong>: {pizza.description}</div>
                        <div>
                            <button onClick={() => handleEdit(pizza)}>Edit</button>
                            <button onClick={() => onDelete(pizza.id)}>Delete</button>
                        </div>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default PizzaList;