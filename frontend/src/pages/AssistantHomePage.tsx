import { useEffect, useState } from 'react'
import ConsentPanel, { type ConsentItem } from '../components/ConsentPanel'
import TrustIndicator, { type TrustIndicatorModel } from '../components/TrustIndicator'
import {
  getConsentProfile,
  getTrustIndicator,
  updateConsent,
  type ConsentProfile,
  type TrustIndicatorResponse,
} from '../services/apiClient'

type AssistantHomePageProps = {
  userId: string
}

const toConsentItems = (profile: ConsentProfile): ConsentItem[] => profile.consents

const toTrustIndicatorModel = (indicator: TrustIndicatorResponse): TrustIndicatorModel => ({
  userId: indicator.userId,
  readOnlyMode: indicator.readOnlyMode,
  readableScopes: indicator.readableScopes,
  blockedScopes: indicator.blockedScopes,
  lastConsentUpdateAt: indicator.lastConsentUpdateAt,
})

function AssistantHomePage({ userId }: AssistantHomePageProps) {
  const [consents, setConsents] = useState<ConsentItem[]>([])
  const [indicator, setIndicator] = useState<TrustIndicatorModel | null>(null)

  useEffect(() => {
    void (async () => {
      const profile = await getConsentProfile(userId)
      setConsents(toConsentItems(profile))

      const trustState = await getTrustIndicator(userId)
      setIndicator(toTrustIndicatorModel(trustState))
    })()
  }, [userId])

  const handleUpdateConsent = async (
    scopeCategory: string,
    status: 'granted' | 'denied' | 'revoked',
  ) => {
    await updateConsent(userId, scopeCategory, status)

    const profile = await getConsentProfile(userId)
    setConsents(toConsentItems(profile))

    const trustState = await getTrustIndicator(userId)
    setIndicator(toTrustIndicatorModel(trustState))
  }

  return (
    <div className="assistant-home-page">
      {indicator ? <TrustIndicator indicator={indicator} /> : <p>Loading trust state...</p>}
      <ConsentPanel consents={consents} onUpdateConsent={handleUpdateConsent} />
    </div>
  )
}

export default AssistantHomePage
