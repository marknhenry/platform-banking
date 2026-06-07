import { expect, test } from '@playwright/test'

test('sign-in context plus consent revoke path keeps read-only trust state', async ({ page }) => {
  await page.goto('/')

  await expect(page.getByRole('heading', { name: /welcome to platform banking/i })).toBeVisible()
  await expect(page.getByRole('heading', { name: /trust indicator/i })).toBeVisible()

  const firstRevokeButton = page.getByRole('button', { name: /^revoked$/i }).first()
  await firstRevokeButton.click()

  await expect(page.getByText(/blocked scopes:/i)).toBeVisible()
  await expect(page.getByText(/mode:\s*read-only/i)).toBeVisible()
})
