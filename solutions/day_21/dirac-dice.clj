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

(def p1-start (nth starting-positions 0))
(def p2-start (nth starting-positions 1))

;; Shared functions
(def get-position
  (memoize (fn [start-position roll]
             (+ (rem (- (+ start-position roll) 1) 10) 1))))

(defn move [player roll]
  (let [new-position   (get-position (:pos player) roll)
        new-score      (+ (:score player) new-position)]
    {:pos new-position :score new-score}))

(defn has-won [player winning-score]
  (>= (:score player) winning-score))

;; Part One
(def get-deterministic-rolls
  (memoize (fn [start-roll]
             (->> (range 0 3)
                  (map (fn [x] (+ start-roll x)))
                  (map (fn [x] (+ (rem (- x 1) 100) 1)))))))

(defn play-game [p1 p2 starting-roll roll-counter]
  (let [roll-numbers  (get-deterministic-rolls starting-roll)
        roll          (reduce + roll-numbers)
        moved-p1      (move p1 roll)
        ;; Input values for next turn
        roll-counter  (+ roll-counter 3)
        starting-roll (+ (last roll-numbers) 1)]
    (if (has-won moved-p1 1000)
      [(:score p2) roll-counter]
      (recur p2 moved-p1 starting-roll roll-counter))))

(defn get-part-one-result [p1-start p2-start]
  (let [p1             {:pos p1-start :score 0}
        p2             {:pos p2-start :score 0}
        [losing-score 
         roll-counter] (play-game p1 p2 1 0)]
    (* losing-score roll-counter)))

(println (get-part-one-result p1-start p2-start))

;; Part Two
(def roll-frequencies
  (frequencies (for [n1 [1 2 3] 
                     n2 [1 2 3] 
                     n3 [1 2 3]] 
                 (+ n1 n2 n3))))

(defn get-total-wins [[t1 t2] [v1 v2]]
  [(+ t1 v1) (+ t2 v2)])

(def count-wins
  (memoize (fn [p1 p2]
             (if (has-won p2 21)
               [0 1]
               (let [wins (for [[roll freq] roll-frequencies
                                :let [moved-p1          (move p1 roll)
                                      [p1-wins p2-wins] (count-wins p2 moved-p1)]]
                            [(* p2-wins freq)
                             (* p1-wins freq)])]
                 (reduce get-total-wins wins))))))

(defn get-part-two-result [p1-start p2-start]
  (let [p1         {:pos p1-start :score 0}
        p2         {:pos p2-start :score 0}
        win-counts (count-wins p1 p2)]
    (apply max win-counts)))

(println (get-part-two-result p1-start p2-start))