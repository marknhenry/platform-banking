export type TrustIndicatorModel = {
  userId: string
  readOnlyMode: boolean
  readableScopes: string[]
  blockedScopes: string[]
  lastConsentUpdateAt: string
}

type TrustIndicatorProps = {
  indicator: TrustIndicatorModel
}

function TrustIndicator({ indicator }: TrustIndicatorProps) {
  return (
    <section className="trust-indicator" aria-label="Trust indicator">
      <h2>Trust Indicator</h2>
      <p>
        User: <strong>{indicator.userId}</strong>
      </p>
      <p>
        Mode: <strong>{indicator.readOnlyMode ? 'Read-only' : 'Unknown'}</strong>
      </p>
      <p>
        Readable scopes: {indicator.readableScopes.length ? indicator.readableScopes.join(', ') : 'none'}
      </p>
      <p>
        Blocked scopes: {indicator.blockedScopes.length ? indicator.blockedScopes.join(', ') : 'none'}
      </p>
      <p className="subtle">Updated {new Date(indicator.lastConsentUpdateAt).toLocaleTimeString()}</p>
    </section>
  )
}

export default TrustIndicator
