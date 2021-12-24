(ns dirac-dice
  (:require [clojure.string :as string]))

(def file-name "C:\\Users\\david\\source\\repos\\advent-of-code-2021\\input\\day_21.txt")
(def input-data (string/split-lines (slurp file-name)))

;; Input data
(defn get-starting-position [input]
  (-> input
      (string/split #":")
      (last)
      (string/trim)
      (Integer/parseInt)))

(def starting-positions
  (->> input-data
       (map get-starting-position)))

;; Shared functions
(defn get-deterministic-rolls [start]
  (->> (range 0 3)
       (map (fn [x] (+ start x)))
       (map (fn [x] (+ (rem (- x 1) 100) 1)))))

(defn get-position [start roll-total]
  (+ (rem (- (+ start roll-total) 1) 10) 1))

(defn move-player [player roll-total]
  (let [new-position   (get-position (:pos player) roll-total)
        new-score      (+ (:score player) new-position)]
    (as-> player p
      (assoc p :pos new-position)
      (assoc p :score new-score))))

(defn has-won [player threshold]
  (>= (:score player) threshold))

;; Part One
(defn play-game [playing waiting starting-roll total-rolls]
  (let [rolls         (get-deterministic-rolls starting-roll)
        moved-player  (move-player playing (reduce + rolls))
        ;; Get input values for next turn
        total-rolls   (+ total-rolls 3)
        starting-roll (+ (last rolls) 1)]
    (cond
      (has-won moved-player 1000) [(:score waiting) total-rolls]
      :else (recur waiting moved-player starting-roll total-rolls))))

(defn get-part-one-result [start-one start-two]
  (let [player-one                 {:pos start-one :score 0}
        player-two                 {:pos start-two :score 0}
        [losing-score total-rolls] (play-game player-one player-two 1 0)]
    (* losing-score total-rolls)))

(println (get-part-one-result (first starting-positions) (last starting-positions)))