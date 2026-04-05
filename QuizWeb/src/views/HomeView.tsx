import { navigate } from '../router'

export function HomeView({ onOpenDashboard }: { onOpenDashboard: () => void }) {
  return (
    <section className="hero-grid">
      <div className="hero-copy panel">
        <p className="eyebrow">REST alapfeladat kész frontenddel</p>
        <h1>Kahoot-szerű kvízélmény egyetlen React webappban.</h1>
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
            Megnyitom az admin részt
          </button>
        </div>
      </div>

      <div className="hero-side">
        <div className="metric-card panel">
          <span className="metric-label">Két nézet</span>
          <strong>Admin + Participant</strong>
          <p>Egyetlen SPA-n belül, külön route-okkal.</p>
        </div>
        <div className="metric-card panel">
          <span className="metric-label">Frissítés</span>
          <strong>Polling alapú state</strong>
          <p>SignalR nélkül is követi az aktuális kérdést és eredményeket.</p>
        </div>
        <div className="metric-card panel">
          <span className="metric-label">Fókusz</span>
          <strong>2 pontos alapfeladat</strong>
          <p>Belépés, kvíz létrehozás, session vezérlés és résztvevői játék.</p>
        </div>
      </div>
    </section>
  )
}
