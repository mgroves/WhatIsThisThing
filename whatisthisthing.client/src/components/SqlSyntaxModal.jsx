import React, { useRef } from 'react';
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter';
import { coy } from 'react-syntax-highlighter/dist/esm/styles/prism';

const SqlSyntaxModal = ({ modalTitle, modalBody }) => {
    // Reference for the code block to use for copying
    const codeRef = useRef(null);

    // Function to handle copying code to the clipboard
    const handleCopy = () => {
        // Copy the plain text content of modalBody
        const codeContent = modalBody;

        // Write the plain text content to the clipboard
        navigator.clipboard.writeText(codeContent).then(() => {
            alert('Copied to clipboard!');
        }).catch(err => {
            console.error('Failed to copy!', err);
        });
    };

    return (
        <div className="modal fade" id="exampleModal" tabIndex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div className="modal-dialog modal-xl">
                <div className="modal-content">
                    <div className="modal-header">
                        <h1 className="modal-title fs-5" id="exampleModalLabel">{modalTitle}</h1>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        <pre ref={codeRef}>
                            <SyntaxHighlighter
                                language="sql"
                                style={coy}
                                wrapLongLines={true}
                                useInlineStyles={true}
                                showLineNumbers={true}
                            >
                                {modalBody}
                            </SyntaxHighlighter>
                        </pre>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">✖ Close</button>
                        <button type="button" className="btn btn-primary" onClick={handleCopy}>📋 Copy</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default SqlSyntaxModal;
