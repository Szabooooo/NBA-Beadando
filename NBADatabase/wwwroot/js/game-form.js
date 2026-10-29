// Game Create/Edit form: loads a team's roster into its box score and keeps the team score in sync
document.addEventListener('DOMContentLoaded', () => {
    const container = side => document.querySelector(`.box-score-container[data-side="${side}"]`);

    // Selecting a team replaces that side's box score with the team's players
    document.querySelectorAll('.team-select').forEach(select => {
        select.addEventListener('change', async () => {
            const side = select.dataset.side;
            if (!select.value) {
                container(side).innerHTML = '';
                return;
            }
            const url = `${window.location.pathname}?handler=Lines&teamId=${select.value}&side=${side}`;
            const response = await fetch(url);
            container(side).innerHTML = await response.text();
        });
    });

    // Typing a stat marks the player as played, and points are added up into the team score
    document.querySelectorAll('.box-score-container').forEach(box => {
        box.addEventListener('input', e => {
            const row = e.target.closest('tr');
            if (e.target.classList.contains('stat') && e.target.value !== '') {
                row.querySelector('.played').checked = true;
            }
            if (/\.(FieldGoalsMade|ThreePointersMade|FreeThrowsMade)$/.test(e.target.name)) {
                updatePoints(row);
            }
            updateScore(box.dataset.side);
        });
        box.addEventListener('change', e => {
            if (e.target.classList.contains('played')) {
                updateScore(box.dataset.side);
            }
        });
    });

    // Points always equal 2 x FGM + 3PM + FTM (the server checks the same rule)
    function updatePoints(row) {
        const made = stat => parseInt(row.querySelector(`input[name$=".${stat}"]`).value) || 0;
        row.querySelector('.points').value = 2 * made('FieldGoalsMade') + made('ThreePointersMade') + made('FreeThrowsMade');
    }

    function updateScore(side) {
        let total = 0;
        let anyPlayed = false;
        container(side).querySelectorAll('tbody tr').forEach(row => {
            const played = row.querySelector('.played');
            if (played && played.checked) {
                anyPlayed = true;
                total += parseInt(row.querySelector('.points').value) || 0;
            }
        });
        if (anyPlayed) {
            document.querySelector(`.team-points[data-side="${side}"]`).value = total;
        }
    }
});
