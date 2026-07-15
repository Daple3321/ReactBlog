import { useState } from 'react';
import { authorizedFetch } from '../blogTools.jsx';

export default function EditBlogForm({ blogToEdit, onUpdated, token }) {
    const [error, setError] = useState(null);

    async function updateBlogRequest(formData) {
        setError(null);
        try {
            const response = await authorizedFetch(
                `/blogs/${blogToEdit.id}`,
                token,
                { method: 'PUT', body: formData },
            );
            if (response.ok) {
                onUpdated(await response.json());
            } else {
                setError(`Server error: ${response.status}`);
            }
        } catch (e) {
            setError(`Request failed: ${e.message}`);
        }
    }

    return (
        <div>
            {error && <p style={{ color: 'salmon' }}>{error}</p>}
            <form action={updateBlogRequest}>
                <p>
                    <input name="name" type="text" placeholder='Blog title' defaultValue={blogToEdit.name} required />
                </p>
                <p>
                    <textarea name="content" rows='15' cols='140' defaultValue={blogToEdit.content} />
                </p>
                <input type='submit' value="Update" />
            </form>
        </div>
    );
}
