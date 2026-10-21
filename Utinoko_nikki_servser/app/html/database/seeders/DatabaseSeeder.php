<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    use WithoutModelEvents;

    public function run(): void
    {
        $this->call([
            ElderUserSeeder::class,
        ]);
        $this->call([
            GamesSeeder::class,
        ]);
        $this->call([
            ItemsSeeder::class,
        ]);
        $this->call([
            AnimalsSeeder::class,
        ]);
        $this->call([
            UserpetsSeeder::class,
        ]);
    }

}
