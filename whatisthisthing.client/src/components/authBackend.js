export async function verifyToken(credential) {
    try {
        const response = await fetch('/api/verify-token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ credential })
        });

        const result = await response.json();

        if (!response.ok) {
            throw new Error(result.message || 'Unknown error');
        }

        return result;
    } catch (error) {
        throw new Error(`Error during token validation: ${error.message}`);
    }
}
