import { useState } from 'react';
import { authorizedFetch } from '../blogTools.jsx';

export default function NewBlogForm({ onCreated, token }) {
    const [error, setError] = useState(null);

    async function postRequest(formData) {
        setError(null);
        try {
            const response = await authorizedFetch('/blogs', token, { method: 'POST', body: formData });
            if (response.ok) {
                onCreated(await response.json());
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
            <form className="new-blog-form" action={postRequest}>
                <input name="name" type="text" placeholder="Blog title" required />
                <textarea name="content" placeholder="Write your blog content here..." />
                <input type="submit" value="Create" />
            </form>
        </div>
    );
}
