import { useEffect, useState } from 'react';
import { getUsers } from '../blogTools.jsx';

export default function Discover({ token, onUserClicked }) {
    const [users, setUsers] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        let cancelled = false;

        getUsers(token)
            .then(result => {
                if (!cancelled) setUsers(result.items);
            })
            .catch(e => {
                if (!cancelled) setError(e.message);
            });

        return () => { cancelled = true; };
    }, [token]);

    if (error) return <p className="error-message">Failed to load users: {error}</p>;
    if (users === null) return <p>Loading users...</p>;

    return (
        <section>
            <h1>Discover users</h1>
            {users.length === 0
                ? <p>No users found.</p>
                : (
                    <ul className="user-list">
                        {users.map(user => (
                            <li key={user.id}>
                                <button className="user-link" onClick={() => onUserClicked(user.username)}>
                                    {user.username}
                                </button>
                            </li>
                        ))}
                    </ul>
                )}
        </section>
    );
}
