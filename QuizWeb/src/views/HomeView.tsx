import { navigate } from '../router'

export function HomeView({ onOpenDashboard }: { onOpenDashboard: () => void }) {
  return (
    <section className="hero-grid">
      <div className="hero-copy panel">
        <p className="eyebrow">MWF Beadandó feladat 2026</p>
        <h2>Kahoot-szerű webalkalmazás.</h2>
        <p className="hero-lead">
          Az admin felületen saját kvízt készíthetsz és indíthatsz, a résztvevők pedig
          PIN kóddal csatlakozhatnak, válaszolhatnak és láthatják az adott kérdés
          eredményét.
        </p>
        <div className="hero-actions">
          <button className="primary-button" onClick={() => navigate('/join')}>
            Belépek játékosként
          </button>
          <button className="secondary-button" onClick={onOpenDashboard}>
            Megnézem a kvízeimet
          </button>
        </div>
      </div>
    </section>
  )
}
