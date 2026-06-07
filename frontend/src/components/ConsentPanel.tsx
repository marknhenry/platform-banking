export type ConsentItem = {
  scopeCategory: string
  status: string
  effectiveAt: string
}

type ConsentPanelProps = {
  consents: ConsentItem[]
  onUpdateConsent: (scopeCategory: string, status: 'granted' | 'denied' | 'revoked') => void
}

const statuses: Array<'granted' | 'denied' | 'revoked'> = ['granted', 'denied', 'revoked']

function ConsentPanel({ consents, onUpdateConsent }: ConsentPanelProps) {
  return (
    <section className="consent-panel" aria-label="Consent management">
      <h2>Consent Preferences</h2>
      <ul>
        {consents.map((consent) => (
          <li key={consent.scopeCategory}>
            <div>
              <strong>{consent.scopeCategory}</strong>
              <span>Status: {consent.status}</span>
            </div>
            <div className="consent-actions">
              {statuses.map((status) => (
                <button
                  key={status}
                  type="button"
                  onClick={() => onUpdateConsent(consent.scopeCategory, status)}
                >
                  {status}
                </button>
              ))}
            </div>
          </li>
        ))}
      </ul>
    </section>
  )
}

export default ConsentPanel
